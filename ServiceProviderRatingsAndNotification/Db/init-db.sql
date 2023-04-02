create table ServiceProvider
(
    Id   uniqueidentifier default newid() not null
        primary key,
    Name nvarchar(20)                     not null
)

create table ServiceProviderRating
(
    ServiceProviderId uniqueidentifier
        references ServiceProvider,
    Rating            tinyint
)

INSERT INTO ServiceProvider (Id, Name) VALUES (N'F4610D85-0EB2-464D-A8F9-04F1FCFBA694', N'Cool SP');
INSERT INTO ServiceProvider (Id, Name) VALUES (N'C9DA4F99-40E9-484C-BBD5-477C016F9726', N'Apulia Sp SRL');

INSERT INTO ServiceProviderRating (ServiceProviderId, Rating) VALUES (N'C9DA4F99-40E9-484C-BBD5-477C016F9726', 5);
INSERT INTO ServiceProviderRating (ServiceProviderId, Rating) VALUES (N'C9DA4F99-40E9-484C-BBD5-477C016F9726', 4);
INSERT INTO ServiceProviderRating (ServiceProviderId, Rating) VALUES (N'F4610D85-0EB2-464D-A8F9-04F1FCFBA694', 3);
INSERT INTO ServiceProviderRating (ServiceProviderId, Rating) VALUES (N'F4610D85-0EB2-464D-A8F9-04F1FCFBA694', 3);
