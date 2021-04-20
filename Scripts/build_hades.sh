#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to pull the latest changes and build HADES

cd ..
git pull

dotnet publish ./HADES/HADES.sln -c Release -o ./HADES/bin/Release/publish/

exit $?