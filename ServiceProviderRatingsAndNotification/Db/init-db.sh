/opt/mssql-tools/bin/sqlcmd -U sa -P $1 -Q 'CREATE DATABASE [spratingsdb]'
/opt/mssql-tools/bin/sqlcmd -U sa -P $1 -d 'spratingsdb' -i /src/init-db.sql