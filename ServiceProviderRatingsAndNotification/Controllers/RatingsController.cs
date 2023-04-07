using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProviderRatingsAndNotification.Controllers.Dtos;
using ServiceProviderRatingsAndNotification.Rating;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

namespace ServiceProviderRatingsAndNotification.Controllers
{
    /// <summary>
    /// Submit ratings for Service Providers and check for notifications
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// Get the average rating of a given Service Provider
        /// </summary>
        /// <param name="serviceProviderId">The id of the Service Provider</param>
        /// <returns>The average rating</returns>
        /// <response code="200">When getting the average rating</response>
        /// <response code="400">If the Service Provider does not exist</response>
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

        /// <summary>
        /// Submit a rating for a Service Provider
        /// </summary>
        /// <param name="serviceProviderId">Service Provider's id</param>
        /// <param name="rating">Rating to set</param>
        /// <returns></returns>
        /// <response code="200">If the rating is correctly submitted</response>
        /// <response code="400">If the rating is outside 1-5 range</response>
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

        /// <summary>
        /// Last rating submissions
        /// </summary>
        /// <remarks>
        /// When called it cleans last notifications. Next calls will return empty result
        /// if no new submissions are applied.
        /// </remarks>
        /// <param name="limit">Max number of notifications to retrieve</param>
        /// <returns></returns>
        /// <response code="500">An error occurred while getting last notifications</response>
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
