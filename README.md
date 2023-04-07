Service Provider Ratings and Notifications Assignment
===
This README gives an overview of the implementation choices made for this project, addressing their motivations and limitations.

_Optional tasks achieved:_
- Include OpenAPI (previously known as the Swagger) for rating
service, which outlines the available endpoints and their functionality.
- Integration, or benchmark tests. Using Testcontainers is a plus.



Quick startup
===
___Prerequisite___: `Docker Desktop` installed on local machine (tested on Windows machine only)


To start playing with the Web Api with the environment set up:
- navigate to `ServiceProviderRatingsAndNotification/` web api project folder
- run `docker-compose up --build`

This will spin all needed containers, namely the MSSQL database, RabbitMQ message broker and the web api.

Navigate at http://localhost:5000/swagger/index.html to reach out the **Swagger UI** and interact with the Api.

> ### Debugging with Visual Studio
> It is possible to run the Api from Visual Studio if created containers with docker-compose are running, given that RabbitMQ and the MsSql db listen of default ports.

Project structure choices
===
In order to keep this project as simple as possible a simple project structure has been kept.
In fact, all different layers of the application reside into the main web project. In a real world scenario a separation with a _Clean Architecture_/_Onion_ style would be preferred, with a Core project having the business logic and entities and no dependencies other than the SDK and utils. The principle of "depending on abstractions, not implementations" is followed, meaning that interfaces are declared internally into the Core project but implemented and injected from the outside with the _Dependency Injection_ pattern. 

This would be followed by for example an _Infrastructure_ layer that depends on the Core layer and implements I/O and data sources access by implementing interfaces provided by the Core layer.

In this project these principles are followed but without formally specifying the explained solution structure, it's done within the Web Api project with folders.

_Repository_ design pattern has been adopted in order to abstract away the data access layer for better decoupling and testability.

Domain Services
===

Under the `ServiceProviderRatingsAndNotification` project there are 2 folders (among the others) related to the two main business services:
- ServiceProvider
- Rating

Those folders contain main operations needed to solve the required tasks.

### `ServiceProvider` folder
Contains:
- `IServiceProviderRepository` with its only implementation `ServiceProviderRepository`
- `ServiceProviderService` that runs business logic and depends on the repository 
- `ServiceProvider` which is the entity representing a Service Provider

This service it's pretty simple because for this project manages the entities retrieval only, much of the workload is implemented in the Rating service

### `RatingService` folder
Contains:
- `RatingService` that depends on `IServiceProviderRepository` and `IServiceProviderNotifier`
- `RatingSubmission` that represents a rating submitted for a given Service Provider

### `ServiceProviderNotification` folder
Contains:
- `IServiceProviderNotifier` with its only implementation `ServiceProviderNotifierWithRabbitMq`

> The `ServiceProviderNotifierWithRabbitMq` service manages the notification phase when dealing with rating submissions.
This service provides notification with RabbitMQ.
However, some considerations have to be done, considering the toy context that this projects tries to address.
The "message consumption" part of this service denatures the goal of RabbitMQ a bit.
Using a queue in this way assumes that there's only a single user that acknowledges all queued messages through a REST
endpoint, moreover there could be scalability issues if the number of submissions grows high, like it would in a real world application.
For example, a queue per user could be considered, but this is out of the scope of this project, willing to demonstrate a basic usage of
a message broker like RabbitMQ.


Integration testing with `Testcontainers`
===
Under `ServiceProviderRatingsAndNotification.Tests/Integration` resides all the integration testing related code.

The `Fixture` folder contains what's needed to create the infrastructure the Core code depends on. This naming follows what's proposed by the `xUnit` library that have been used.

Test classes for integration testing have been created as members of their respective "containers" classes, just for the purpose of employing the power of parallel tests execution thanks to Docker containers creation for each test. In this way tests are run way faster!

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
