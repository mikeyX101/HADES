#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to build AND run HADES with profiling. Run with app user

cd /var/www/hades
git pull

dotnet publish ./HADES/HADES.sln -c Release-Profiling -o ./HADES/bin/ReleaseProfiling/publish/

cd /var/www/hades/HADES/
dotnet ./bin/ReleaseProfiling/publish/HADES.dll