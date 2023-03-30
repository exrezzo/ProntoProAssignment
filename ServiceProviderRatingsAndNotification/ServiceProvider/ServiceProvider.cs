namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public class ServiceProvider
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double AverageRating { get; set; }
}

public record Rating {}