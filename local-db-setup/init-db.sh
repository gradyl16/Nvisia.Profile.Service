#!/bin/bash

sleep 30

#run the setup script to create the DB and the schema in the DB
echo "executing sql scripts..."
for i in {1..50};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i ./local-db-setup/schema.sql -i ./local-db-setup/data.sql
    if [ $? -eq 0 ]
    then
        echo "...sql scripts completed"
        break
    else
        echo "not ready to execute sql scripts yet..."
        sleep 1
    fi
done