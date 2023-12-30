#!/bin/bash

sleep 30

#run the setup script to create the DB and the schema in the DB
echo "executing sql scripts..."
for i in {1..50};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -d master -i /var/opt/mssql/schema.sql
    if [ $? -eq 0 ]
    then
        echo "...sql scripts completed"
        break
    else
        echo "not ready to execute sql scripts yet..."
        sleep 1
    fi
done