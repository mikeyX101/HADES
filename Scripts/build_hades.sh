#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to pull the latest changes and build HADES

git pull git@github.com:ShaiLynx/HADES.git ~/hades

dotnet publish ../HADES/HADES.sln -c Release -o ../HADES/bin/Release/publish/

# Execute init migrations (not ready and missing arguments)
#dotnet ef database update

exit $?