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
    public class RepositoriesFixture : IAsyncDisposable
    {
        private readonly MsSqlContainer _container;

        public IServiceProviderRepository ServiceProviderRepository { get; }

        public RepositoriesFixture()
        {
            _container = new MsSqlBuilder().Build();
            _container.StartAsync().Wait();
            var connectionString = _container.GetConnectionString();
            var execScriptAsync = _container.ExecScriptAsync(File.ReadAllText("Integration/Fixture/init-db.sql")).Result;

            ServiceProviderRepository = new ServiceProviderRepository(connectionString);

            
        }

        public async ValueTask DisposeAsync()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}
