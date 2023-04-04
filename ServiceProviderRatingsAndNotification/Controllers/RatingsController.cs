using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProviderRatingsAndNotification.Controllers.Dtos;
using ServiceProviderRatingsAndNotification.Rating;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

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

        [HttpGet("average-rating")]
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

        [HttpGet("submit-rating")]
        public async Task<ActionResult> SubmitRatingAsync([FromQuery] Guid serviceProviderId, [FromQuery] uint rating)
        {
            try
            {
                await _ratingService.SubmitRating(rating, serviceProviderId);
                return Ok();
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("last-rating-submissions")]
        public ActionResult<IEnumerable<RatingSubmission>> GetLastNotificationsAsync([FromQuery] uint limit = 100)
        {
            try
            {
                var submissions = _ratingService.GetLastRatingSubmissions(limit);
                return Ok(submissions);
            }
            catch (InvalidRatingSubmissionMessageException e)
            {
                return new ObjectResult(e.Message)
                {
                    StatusCode = 500
                };
            }
        }
    }
}
