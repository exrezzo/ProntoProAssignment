using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProviderRatingsAndNotification.Controllers.Dtos;
using ServiceProviderRatingsAndNotification.Rating;

namespace ServiceProviderRatingsAndNotification.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }
        [HttpGet]
        public async Task<ActionResult<AverageRatingDto>> GetAverageRatingAsync([FromQuery] Guid serviceProviderId)
        {
            try
            {
                var averageRating = await _ratingService.GetAverageRatingForServiceProvider(serviceProviderId);
                return Ok(new AverageRatingDto()
                {
                    ServiceProviderId = serviceProviderId,
                    AverageRating = averageRating
                });
            }
            catch (ServiceProviderNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
