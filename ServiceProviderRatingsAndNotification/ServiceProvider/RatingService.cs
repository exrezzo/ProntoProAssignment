namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public class RatingService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;

    public RatingService(IServiceProviderRepository serviceProviderRepository)
    {
        _serviceProviderRepository = serviceProviderRepository;
    }

    public async Task<double> GetAverageRatingForServiceProvider(Guid id)
    {
        var serviceProvider = await _serviceProviderRepository.GetAsync(id);
        if (serviceProvider is null)
            throw new ArgumentException($"It does not exist any Service Provider for id {id}");
        var ratings = await _serviceProviderRepository.GetRatingsAsync(id);
        return ratings.Any() ? ratings.Average() : 0;
    }
}