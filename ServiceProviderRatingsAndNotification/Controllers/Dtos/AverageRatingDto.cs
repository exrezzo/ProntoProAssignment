namespace ServiceProviderRatingsAndNotification.Controllers.Dtos
{
    public class AverageRatingDto
    {
        public Guid ServiceProviderId { get; set; }
        public double AverageRating { get; set; }
    }
}
