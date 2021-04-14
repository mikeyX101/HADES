#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to run HADES

dotnet ../HADES/bin/Release/publish/HADES.dll
exit $?