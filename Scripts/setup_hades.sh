#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to prepare an SSH key to get HADES from Github, install needed dependencies and setup the Nginx reverse proxy.

if ! [ $(id -u) = 0 ]; then
   echo "The script need to be run as root." >&2
   exit 1
fi

# -----------------------------------------------------------------
#						www-hades User Setup
# -----------------------------------------------------------------

# Create app user
if ! id www-hades >/dev/null 2>&1; then
	useradd --system www-hades 
fi

# Make site folder
if ! [ -d /var/www ]; then 
	mkdir /var/www
	chown www-hades /var/www
	chgrp www-hades /var/www
fi

# Needed for dotnet
# DotNet creates a .dotnet folder in ~ when users use the DotNet CLI for various operations
if ! [ -d /home/www-hades ]; then 
	mkdir /home/www-hades
	chown www-hades /home/www-hades
	chgrp www-hades /home/www-hades
	chmod 700 /home/www-hades
fi

# -----------------------------------------------------------------
#							git Setup
# -----------------------------------------------------------------
# Generate rsa4096 key without passcode
echo "Generating SSH key for the Github repository..."
ssh-keygen -q -t rsa -b 4096 -N "" -f /var/www/github_hades_rsa <<< n # Answer no if asking to overwrite
chmod 640 /var/www/github_hades_rsa
chmod 644 /var/www/github_hades_rsa.pub
chown www-hades /var/www/github_hades_*
chgrp www-hades /var/www/github_hades_*
echo

# Allow git to use key on Github in SSH config if file or entry doesn't exist
if ! [ -e /etc/ssh/ssh_config ] || ! grep -q "IdentityFile /var/www/github_hades_rsa" /etc/ssh/ssh_config; then
	echo "Modifying server SSH config to allow git to use the SSH key to Github.com..." 
	# Formatted config
	# Host *
	#	Hostname github.com
	#	User git
	#	IdentityFile /var/www/github_hades_rsa
	echo $'Host *\n\tHostname github.com\n\tUser git\n\tIdentityFile /var/www/github_hades_rsa' >> /etc/ssh/ssh_config
	
	# Make sure config is readable
	#chmod 644 ~/.ssh/config
fi

# Install git (-q)
echo "Installing git..."
yum -y install git
# Make sure we use Unix line endings
git config --system core.autocrlf input
git config --global core.autocrlf input

# -----------------------------------------------------------------
#							Nginx Setup
# -----------------------------------------------------------------
# Add Nginx's package repository
echo "Installing Nginx's package repository..."
yum -y install epel-release

# Install Nginx (-q)
echo "Installing Nginx..."
yum -y install nginx

echo "Configuring Nginx..."
# https://wiki.centos.org/TipsAndTricks/SelinuxBooleans
# httpd_can_network_connect (HTTPD Service)
#    Allow HTTPD scripts and modules to connect to the network. 
setsebool -P httpd_can_network_connect 1

firewall-cmd --permanent --zone=public --add-service=http
firewall-cmd --permanent --zone=public --add-service=https


# -----------------------------------------------------------------
#							DotNet Setup
# -----------------------------------------------------------------
# Add Microsoft's CentOS 7 package repository (--quiet)
rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

# Install dotnet-sdk-5.0 (-q)
echo "Installing dotnet-sdk-5.0... (Requires root, can take some time)"
yum -y install dotnet-sdk-5.0

# Opt-out of DotNet telemetry
#if ! grep -q "export DOTNET_CLI_TELEMETRY_OPTOUT=1" ~/.bashrc; then
#	echo $'export DOTNET_CLI_TELEMETRY_OPTOUT=1\n' >> ~/.bashrc
#	source ~/.bashrc
#fi

# -----------------------------------------------------------------
#							HADES Setup
# -----------------------------------------------------------------
echo "Public key available at: /var/www/github_hades_rsa.pub"
echo "Public key: "
cat /var/www/github_hades_rsa.pub

read -n1 -rsp $'\nPlease wait for the key to be added in the git repository and press any key to continue...\n'

echo "Cloning HADES in /var/www/hades..."
if ! [ -d /var/www/hades ]; then 
	mkdir /var/www/hades
	chown www-hades /var/www/hades
	chgrp www-hades /var/www/hades
fi
git clone git@github.com:ShaiLynx/HADES.git /var/www/hades
chown -R www-hades /var/www/hades/*
chgrp -R www-hades /var/www/hades/*
chown -R www-hades /var/www/hades/.git*
chgrp -R www-hades /var/www/hades/.git*



cd /var/www/hades
git checkout SC/server_scripts

# Copy the HADES Kestrel Server service and start it
cp /var/www/hades/Scripts/configs/systemd/hades-kestrel-server.service /etc/systemd/system/hades-kestrel-server.service

# Copy new Nginx config and start Nginx
cp /var/www/hades/Scripts/configs/nginx/nginxHttps.conf /etc/nginx/nginx.conf

echo "Enabling HADES Kestrel Server..."
systemctl enable hades-kestrel-server.service

echo "Enabling Nginx..."
systemctl enable nginx

echo "Please specify your SSL Certificate and your server name in the Nginx config file at: /etc/nginx/nginx.conf and reboot the system." 