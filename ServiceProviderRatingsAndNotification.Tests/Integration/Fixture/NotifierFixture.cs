using ServiceProviderRatingsAndNotification.ServiceProviderNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace ServiceProviderRatingsAndNotification.Tests.Integration.Fixture
{
    public class NotifierFixture : IAsyncDisposable
    {
        private readonly RabbitMqContainer _rabbitMqContainer;
        public IServiceProviderNotifier ServiceProviderNotifier { get; private set; }

        public NotifierFixture()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            _rabbitMqContainer.StartAsync().Wait();
            ServiceProviderNotifier = new ServiceProviderNotifierWithRabbitMq(
                _rabbitMqContainer.Hostname,
                _rabbitMqContainer.GetMappedPublicPort(5672));
        }
        public async ValueTask DisposeAsync()
        {
            await _rabbitMqContainer.StopAsync();
            await _rabbitMqContainer.DisposeAsync();
        }
    }
}
