using Microsoft.AspNetCore.Connections;
using System.Data.Common;
using System.Threading.Channels;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace ServiceProviderRatingsAndNotification.ServiceProviderNotification
{
    public interface IServiceProviderNotifier
    {
        Task NotifyRatingSubmittedAsync(Guid serviceProviderId, uint rating);
        List<string> GetLastRatingSubmissions(uint limit);
    }

    /// <summary>
    /// This service provides notification with RabbitMQ.
    /// However, some considerations have to be done, considering the toy context that this projects tries to address.
    /// The "message consumption" part of this service denatures the goal of RabbitMQ a bit.
    /// Using a queue in this way assumes that there's only a single user that acknowledges all queued messages through a REST
    /// endpoint, moreover there could be scalability issues if the number of submissions grows high, like it would in a real world application.
    /// For example, a queue per user could be considered, but this is out of the scope of this project, willing to demonstrate a basic usage of
    /// a message broker like RabbitMQ.
    /// </summary>
    public class ServiceProviderNotifierWithRabbitMq : IServiceProviderNotifier
    {
        private readonly IModel _channel;
        private const string _queueName = "submittedRatings";

        public ServiceProviderNotifierWithRabbitMq(string hostName, ushort port = 5672)
        {
            var factory = new ConnectionFactory() { HostName = hostName, Port = port};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName, durable: true, autoDelete: false, exclusive:false);
        }

        /// <summary>
        /// Sends a message to the message broker
        /// </summary>
        /// <param name="serviceProviderId">The id of the Service Provider which received a new rating</param>
        /// <param name="rating">The rating between 1 and 5</param>
        /// <returns></returns>
        public Task NotifyRatingSubmittedAsync(Guid serviceProviderId, uint rating)
        {
            if (rating is < 1 or > 5)
                throw new ArgumentOutOfRangeException($"Rating {rating} is not in range 1 to 5");
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _queueName,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes($"{serviceProviderId} {rating}"));
            return Task.CompletedTask;
        }
        /// <summary>
        /// Gets last rating submissions from the message broker and acknowledges them, removing
        /// from the queue.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns>A list of rating submission messages, in the form "00000000-0000-0000-0000-000000000000 1", that is Service provider Id
        /// and the rating</returns>
        public List<string> GetLastRatingSubmissions(uint limit = 100)
        {
            var messages = new List<string>();
            var basicGetResult = _channel.BasicGet(_queueName, autoAck: true);
            while (basicGetResult != null)
            {
                var body = basicGetResult.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messages.Add(message);

                if (messages.Count == limit) break;
                basicGetResult = _channel.BasicGet(_queueName, autoAck: true);
            }
            messages.Reverse();
            return messages;
        }
    }
}
