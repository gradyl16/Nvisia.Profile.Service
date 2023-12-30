#!/bin/bash

set -m
/var/opt/mssql/init-db.sh & /opt/mssql/bin/sqlservr
fg
