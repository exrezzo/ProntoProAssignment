namespace ServiceProviderRatingsAndNotification.Rating
{
    public class RatingSubmission
    {
        public Guid ServiceProviderId { get; set; }
        public uint Rating { get; set; }
    }
}
