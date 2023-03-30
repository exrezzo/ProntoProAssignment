using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

namespace ServiceProviderRatingsAndNotification.Rating;

public class RatingService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IServiceProviderNotifier _serviceProviderNotifier;

    public RatingService(IServiceProviderRepository serviceProviderRepository, IServiceProviderNotifier serviceProviderNotifier)
    {
        _serviceProviderRepository = serviceProviderRepository;
        _serviceProviderNotifier = serviceProviderNotifier;
    }
    /// <summary>
    /// Returns the average rating for a Service Provider, rounded with 2 decimal digit.
    /// </summary>
    /// <param name="serviceProviderId"></param>
    /// <returns></returns>
    public async Task<double> GetAverageRatingForServiceProvider(Guid serviceProviderId)
    {
        await _serviceProviderExists(serviceProviderId);
        var ratings = await _serviceProviderRepository.GetRatingsAsync(serviceProviderId);
        return ratings.Any() ? Math.Round(ratings.Average(), 2) : 0;
    }

    /// <summary>
    /// Submits a valid rating for a given Service Provider
    /// </summary>
    /// <param name="rating">Rating between 1 and 5 included</param>
    /// <param name="serviceProviderId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task SubmitRating(int rating, Guid serviceProviderId)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentOutOfRangeException($"Cannot set rating {rating} because it's out of range 1-5.");
        await _serviceProviderExists(serviceProviderId);
        await _serviceProviderRepository.AddRatingAsync(serviceProviderId, rating);
        await _serviceProviderNotifier.NotifyRatingSubmittedAsync(serviceProviderId, rating);
    }

    private async Task _serviceProviderExists(Guid serviceProviderId)
    {
        var serviceProvider = await _serviceProviderRepository.GetAsync(serviceProviderId);
        if (serviceProvider is null)
            throw new ArgumentException($"It does not exist any Service Provider with id {serviceProviderId}");
    }
}