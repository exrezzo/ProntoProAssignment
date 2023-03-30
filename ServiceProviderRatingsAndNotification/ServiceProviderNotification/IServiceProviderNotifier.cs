namespace ServiceProviderRatingsAndNotification.ServiceProviderNotification
{
    public interface IServiceProviderNotifier
    {
        Task NotifyRatingSubmittedAsync(Guid serviceProviderId, int  rating);
    }
}
