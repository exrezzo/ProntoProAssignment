using ServiceProviderRatingsAndNotification.Rating;

namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public class ServiceProviderService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;

    public ServiceProviderService(IServiceProviderRepository serviceProviderRepository)
    {
        _serviceProviderRepository = serviceProviderRepository;
    }

    public async Task<IEnumerable<ServiceProvider>> GetAllAsync()
    {
        return await _serviceProviderRepository.GetAllAsync();
    }
}

