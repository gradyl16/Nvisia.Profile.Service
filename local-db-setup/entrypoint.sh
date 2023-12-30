#!/bin/bash

set -m
./local-db-setup/init-db.sh & /opt/mssql/bin/sqlservr
fg