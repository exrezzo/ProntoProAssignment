using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProviderRatingsAndNotification.Controllers.Dtos;
using ServiceProviderRatingsAndNotification.Rating;

namespace ServiceProviderRatingsAndNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviderRepository _serviceProviderRepository;

        public ServiceProviderController(IServiceProviderRepository serviceProviderRepository)
        {
            _serviceProviderRepository = serviceProviderRepository;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<ServiceProviderDto>>> GetServiceProvidersAsync()
        {
            var serviceProviders = await _serviceProviderRepository.GetAllAsync();
            return Ok(serviceProviders.Select(sp => new ServiceProviderDto()
            {
                Id = sp.Id,
                Name = sp.Name
            }));
        }
    }
}
