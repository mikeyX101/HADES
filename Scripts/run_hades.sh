#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to run HADES. Run with app user

cd /var/www/hades/HADES/
dotnet ./bin/Release/publish/HADES.dll
exit $?