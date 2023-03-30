namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public interface IServiceProviderRepository
{
    Task<ServiceProvider> GetAsync(Guid id);
    Task<IEnumerable<int>> GetRatingsAsync(Guid id);
}