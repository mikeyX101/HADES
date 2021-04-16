#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to prepare an SSH key to get HADES from Github and install needed dependencies.

# Generate rsa4096 key without passcode
echo "Generating SSH key for the Github repository..."
ssh-keygen -q -t rsa -b 4096 -N "" -f ~/.ssh/github_hades_rsa <<< n # Answer no if asking to overwrite

# Allow git to use key on Github in SSH config if file or entry doesn't exist
if ! [ -e ~/.ssh/config ] || ! grep -q "IdentityFile ~/.ssh/github_hades_rsa" ~/.ssh/config; then
	echo "Modifying user SSH config to allow git to use the SSH key to Github.com..." 
	# Formatted config
	# Host *
	#	Hostname github.com
	#	User git
	#	IdentityFile ~/.ssh/github_hades_rsa
	echo $'Host *\n\tHostname github.com\n\tUser git\n\tIdentityFile ~/.ssh/github_hades_rsa' >> ~/.ssh/config
	
	# Make sure config is readable
	chmod 644 ~/.ssh/config
fi

# Install git (-q)
echo "Installing git... (Requires root)"
sudo yum -y install git


# Add Microsoft's CentOS 7 package repository (--quiet)
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

# Install dotnet-sdk-5.0 (-q)
echo "Installing dotnet-sdk-5.0... (Requires root, can take some time)"
sudo yum -y install dotnet-sdk-5.0

# Opt-out of DotNet telemetry
if ! grep -q "export DOTNET_CLI_TELEMETRY_OPTOUT=1" ~/.bashrc; then
	echo $'export DOTNET_CLI_TELEMETRY_OPTOUT=1\n' >> ~/.bashrc
	source ~/.bashrc
fi

echo "Public key available at: ~/.ssh/github_hades_rsa.pub"
echo "Public key: "
cat ~/.ssh/github_hades_rsa.pub

read -n1 -rsp $'\nPlease wait for the key to be added in the git repository and press any key to continue...\n'

echo "Cloning HADES in ~/hades..."
if ! [ -d ~/hades ]; then 
	mkdir ~/hades
fi
git clone git@github.com:ShaiLynx/HADES.git ~/hades

# Make deploy scripts executable
if [ -d ~/hades/Scripts ]; then 
	chmod 755 ~/hades/Scripts/*.sh
fi