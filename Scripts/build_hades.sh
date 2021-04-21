#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to pull the latest changes and build HADES

cd ~/hades
git pull
chmod 755 ./Scripts/*.sh

dotnet publish ./HADES/HADES.sln -c Release -o ./HADES/bin/Release/publish/

exit $?