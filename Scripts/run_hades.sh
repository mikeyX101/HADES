#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to run HADES

cd ~/hades/HADES
dotnet ./bin/Release/publish/HADES.dll
exit $?