using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProviderRatingsAndNotification.Controllers.Dtos;
using ServiceProviderRatingsAndNotification.Rating;
using ServiceProviderRatingsAndNotification.ServiceProvider;

namespace ServiceProviderRatingsAndNotification.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly ServiceProviderService _serviceProviderService;

        public ServiceProviderController(ServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<ServiceProviderDto>>> GetServiceProvidersAsync()
        {
            var serviceProviders = await _serviceProviderService.GetAllAsync();
            return Ok(serviceProviders.Select(sp => new ServiceProviderDto()
            {
                Id = sp.Id,
                Name = sp.Name
            }));
        }
    }
}
