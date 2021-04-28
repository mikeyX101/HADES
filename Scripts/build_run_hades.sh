#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to build AND run HADES
bash ./build_hades.sh
ret=$?
if [ $ret -ne 0 ]; then
	exit 1;
fi

bash ./run_hades.sh
exit $?
