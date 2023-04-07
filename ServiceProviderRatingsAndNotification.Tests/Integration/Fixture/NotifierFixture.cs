using ServiceProviderRatingsAndNotification.ServiceProviderNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace ServiceProviderRatingsAndNotification.Tests.Integration.Fixture
{
    public class NotifierFixture : IAsyncLifetime
    {
        private RabbitMqContainer _rabbitMqContainer;
        public IServiceProviderNotifier ServiceProviderNotifier { get; private set; }

        public async Task InitializeAsync()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            await _rabbitMqContainer.StartAsync();
            ServiceProviderNotifier = new ServiceProviderNotifierWithRabbitMq(
                _rabbitMqContainer.Hostname,
                _rabbitMqContainer.GetMappedPublicPort(5672));
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _rabbitMqContainer.StopAsync();
            await _rabbitMqContainer.DisposeAsync();
        }
    }
}
