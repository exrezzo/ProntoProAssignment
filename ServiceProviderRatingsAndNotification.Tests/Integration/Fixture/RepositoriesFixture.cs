using ServiceProviderRatingsAndNotification.ServiceProvider;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace ServiceProviderRatingsAndNotification.Tests.Integration.Fixture
{
    public class RepositoriesFixture : IAsyncLifetime
    {
        private MsSqlContainer _container;

        public IServiceProviderRepository ServiceProviderRepository { get; private set; }

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder().Build();
            await _container.StartAsync();
            var connectionString = _container.GetConnectionString();
            var execScriptAsync = await _container.ExecScriptAsync(File.ReadAllText("Integration/Fixture/init-db.sql"));

            ServiceProviderRepository = new ServiceProviderRepository(connectionString);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }

    }
}
