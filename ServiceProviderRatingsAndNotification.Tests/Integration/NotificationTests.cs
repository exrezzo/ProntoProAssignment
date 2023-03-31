using ServiceProviderRatingsAndNotification.ServiceProviderNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ServiceProviderRatingsAndNotification.Tests.Integration
{
    public class NotificationTests
    {
        /*
         * This test should be improved with a dedicated RabbitMQ container, because using a common one could
         * return dirty data, maybe in between could lie other submissions from other executions, making fail the test.
         * Testcontainers package usage should be evaluated.
         */
        [Fact]
        public async Task Rating_Submissions_Are_Sent_And_Retrieved_From_RabbitMQ()
        {
            var notifier = new ServiceProviderNotifierWithRabbitMq("localhost");

            // some GUIDs are generated to simulate new Service Providers that received a rating 
            var guids = Enumerable.Range(0, 10).Select(i => new Guid(i, 0, 0, new byte[8])).ToList();

            // notify a rating of 1 for those fake Service Providers
            foreach (var guid in guids)
                await notifier.NotifyRatingSubmittedAsync(guid, 1);

            // we get them from RabbitMQ, but are reversed because we want to check if messages have been sent all correctly
            // regardless the timing when happened.
            var lastRatingSubmissions = notifier.GetLastRatingSubmissions();
            lastRatingSubmissions.Reverse();

            foreach (var (submissionMsg, guid) in lastRatingSubmissions.Zip(guids))
            {
                var guidStrFromMsg = submissionMsg.Split(' ')[0];
                var guidFromMsg = Guid.Parse(guidStrFromMsg);

                // we should get the same GUIDs for which a rating have been submitted
                guidFromMsg.Should().Be(guid);
            }
        }

    }
}
