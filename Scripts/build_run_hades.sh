#!/bin/bash
# Samuel Caron

# Tested on CentOS 7.9.2009
# Script to build AND run HADES. Run with app user
bash /var/www/hades/Scripts/build_hades.sh
ret=$?
if [ $ret -ne 0 ]; then
	exit 1;
fi

bash /var/www/hades/Scripts/run_hades.sh
exit $?
