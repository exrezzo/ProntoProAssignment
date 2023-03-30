namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public class ServiceProviderService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;

    public ServiceProviderService(IServiceProviderRepository serviceProviderRepository)
    {
        _serviceProviderRepository = serviceProviderRepository;
    }
    
    
}

