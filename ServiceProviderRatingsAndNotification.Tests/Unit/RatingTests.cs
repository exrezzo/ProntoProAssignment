using FluentAssertions;
using Moq;
using ServiceProviderRatingsAndNotification.Rating;
using ServiceProviderRatingsAndNotification.ServiceProvider;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

namespace ServiceProviderRatingsAndNotification.Tests.Unit;

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
        var ratingService = new RatingService(serviceProviderRepoMock.Object, Mock.Of<IServiceProviderNotifier>());
        var averageRating = await ratingService.GetAverageRatingForServiceProvider(id);
        averageRating.Should().Be(expectedAverage);
    }

    [Fact]
    public async Task Getting_Average_Rating_For_Non_Existing_Service_Provider_Throws()
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
            .ReturnsAsync(new []{1});

        var ratingService = new RatingService(serviceProviderRepoMock.Object, Mock.Of<IServiceProviderNotifier>());
        var averageRatingFunc = async () => await ratingService.GetAverageRatingForServiceProvider(Guid.NewGuid());

        await averageRatingFunc.Should().ThrowExactlyAsync<ServiceProviderNotFoundException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Adding_Invalid_Rating_Throws(uint invalidRating)
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
        var ratingService = new RatingService(serviceProviderRepoMock.Object, Mock.Of<IServiceProviderNotifier>());
        var throwingSubmission = async () =>
        {
            await ratingService.SubmitRating(invalidRating, id);
        };

        await throwingSubmission.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Rating_Submission_Works_Correctly()
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
        var ratingService = new RatingService(serviceProviderRepoMock.Object, Mock.Of<IServiceProviderNotifier>());
        var possibleRatings = Enumerable.Range(1, 5).Select(i => (uint) i);

       possibleRatings
            .Select<uint, Func<Task>>(rating => 
                async () => await ratingService.SubmitRating(rating, id))
            .Should()
            .AllSatisfy(func => 
                func.Should().NotThrowAsync("Acceptable ratings are in the range 1 to 5"));
    }

    [Fact]
    public void Getting_Last_Submitted_Ratings()
    {
        var serviceProviderRepoMock = new Mock<IServiceProviderRepository>();
        var serviceProviderNotifierMock = new Mock<IServiceProviderNotifier>();

        serviceProviderNotifierMock
            .Setup(notifier =>
                notifier.GetLastRatingSubmissions(It.IsAny<uint>()))
            .Returns(new List<string>() { "00000000-0000-0000-0000-000000000000 1" });

        var ratingService = new RatingService(serviceProviderRepoMock.Object, serviceProviderNotifierMock.Object);


        var lastRatingSubmissions = ratingService.GetLastRatingSubmissions(1);
        lastRatingSubmissions
            .First()
            .Should()
            .BeEquivalentTo(new RatingSubmission()
            {
                ServiceProviderId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                Rating = 1
            });
    }

    [Fact]
    public void Getting_A_Malformed_Rating_Submission_Message_Should_Throw()
    {
        var serviceProviderRepoMock = new Mock<IServiceProviderRepository>();
        var serviceProviderNotifierMock = new Mock<IServiceProviderNotifier>();

        serviceProviderNotifierMock
            .Setup(notifier =>
                notifier.GetLastRatingSubmissions(It.IsAny<uint>()))
            .Returns(new List<string>() { "00000000-invalid-Guid 1992" });

        var ratingService = new RatingService(serviceProviderRepoMock.Object, serviceProviderNotifierMock.Object);


        var submissionsGettingFunc = () => ratingService.GetLastRatingSubmissions(1).ToList();
        submissionsGettingFunc.Should().ThrowExactly<InvalidRatingSubmissionMessageException>();
    }
}