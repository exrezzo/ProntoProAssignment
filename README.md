# Service Provider Ratings and Notifications Assignment
___by Ivan Ardillo___ 
## Introduction
This README gives an overview of the implementation choices made for this project, addressing their motivations and limitations.

## Domain Services
...

## REST Api
...

## Environment containerization
In order to keep a consisted and replicable environment via **Docker** a `docker-compose.yml` file have been created.
Please run 
```
docker-compose build 
docker-compose run
```
or `docker-compose run --build` to set up a working environment with all needed dependencies.
### MSSQL db with schema and data preloaded
This web api relies on a MSSQL database and RabbitMQ as a message broker. While the RabbitMQ image is used as-is, the db image has been customized in order to build it containing already the schema and some data.
This have been achieved by using:
- A Dockerfile in `/Db/Dockerfile`
- An `init-db.sql` script that contains schema definition and data insertion
- An `init-db.sh` bash script that is invoked from the Dockerfile while building the image

By now the only working way to seed data at this stage is by waiting for the mssql db to start and then execute the `init-db.sh` script:
```
RUN ( /opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && /src/init-db.sh SuperPass92! \
    && pkill sqlservr
```
This assures correct schema and data initialization avoiding to use a timer that could waste time and is not reliable given the unknown environment where this could be deployed, having for example low cpu capacity.
