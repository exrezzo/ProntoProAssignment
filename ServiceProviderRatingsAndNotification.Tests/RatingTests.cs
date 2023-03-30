using FluentAssertions;
using Moq;
using ServiceProviderRatingsAndNotification.ServiceProvider;

namespace ServiceProviderRatingsAndNotification.Tests;

public class RatingTests
{
    [Theory]
    [InlineData(new int[]{}, 0)]
    [InlineData(new []{1, 5}, 3)]
    [InlineData(new []{2, 5}, 3.5)]
    public async Task Average_Rating_For_A_Service_Provider(IEnumerable<int> ratings, double expectedAverage)
    {
        var serviceProviderRepoMock = new Mock<IServiceProviderRepository>();
        var id = Guid.NewGuid();
        serviceProviderRepoMock
            .Setup(repository => repository.GetAsync(id))
            .ReturnsAsync(new ServiceProvider.ServiceProvider()
            {
                Id = id,
                Name = "Cool Service Provider"
            });
        serviceProviderRepoMock
            .Setup(repository =>
                repository.GetRatingsAsync(id))
            .ReturnsAsync(ratings);
        var ratingService = new RatingService(serviceProviderRepoMock.Object);
        var averageRating = await ratingService.GetAverageRatingForServiceProvider(id);
        averageRating.Should().Be(expectedAverage);
    }
}