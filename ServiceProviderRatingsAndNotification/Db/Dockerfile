FROM mcr.microsoft.com/mssql/server:2019-latest

COPY ./Db/init-db.sh /src/
COPY ./Db/init-db.sql /src/
ENV ACCEPT_EULA Y
ENV SA_PASSWORD SuperPass92!

RUN ( /opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && /src/init-db.sh SuperPass92! \
    && pkill sqlservr

