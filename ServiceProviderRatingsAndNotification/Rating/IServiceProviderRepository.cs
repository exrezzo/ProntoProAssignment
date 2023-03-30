namespace ServiceProviderRatingsAndNotification.Rating;

public interface IServiceProviderRepository
{
    Task<ServiceProvider.ServiceProvider> GetAsync(Guid serviceProviderId);
    Task<IEnumerable<int>> GetRatingsAsync(Guid serviceProviderId);
    Task AddRatingAsync(Guid serviceProviderId, int rating);
}