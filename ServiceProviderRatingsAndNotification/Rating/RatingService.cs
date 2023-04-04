using System.Text.RegularExpressions;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

namespace ServiceProviderRatingsAndNotification.Rating;

public class RatingService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IServiceProviderNotifier _serviceProviderNotifier;

    public RatingService(IServiceProviderRepository serviceProviderRepository, IServiceProviderNotifier serviceProviderNotifier)
    {
        _serviceProviderRepository = serviceProviderRepository;
        _serviceProviderNotifier = serviceProviderNotifier;
    }
    /// <summary>
    /// Returns the average rating for a Service Provider, rounded with 2 decimal digit.
    /// </summary>
    /// <param name="serviceProviderId"></param>
    /// <returns>Two decimal digit precision average rating</returns>
    public async Task<double> GetAverageRatingForServiceProvider(Guid serviceProviderId)
    {
        await _serviceProviderExists(serviceProviderId);
        var ratings = await _serviceProviderRepository.GetRatingsAsync(serviceProviderId);
        return ratings.Any() ? Math.Round(ratings.Average(), 2) : 0;
    }

    /// <summary>
    /// Submits a valid rating for a given Service Provider
    /// </summary>
    /// <param name="rating">Rating between 1 and 5 included</param>
    /// <param name="serviceProviderId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task SubmitRating(uint rating, Guid serviceProviderId)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentOutOfRangeException($"Cannot set rating {rating} because it's out of range 1-5.");
        await _serviceProviderExists(serviceProviderId);
        await _serviceProviderRepository.AddRatingAsync(serviceProviderId, rating);
        await _serviceProviderNotifier.NotifyRatingSubmittedAsync(serviceProviderId, rating);
    }

    public IEnumerable<RatingSubmission> GetLastRatingSubmissions(uint limit)
    {
        var lastSubmissionsMessages = _serviceProviderNotifier.GetLastRatingSubmissions(limit);
        var submissions = lastSubmissionsMessages.Select(_parseRatingSubmissionMessage);
        return submissions;
    }

    private async Task _serviceProviderExists(Guid serviceProviderId)
    {
        var serviceProvider = await _serviceProviderRepository.GetAsync(serviceProviderId);
        if (serviceProvider is null)
            throw new ServiceProviderNotFoundException($"It does not exist any Service Provider with id {serviceProviderId}");
    }

    private RatingSubmission _parseRatingSubmissionMessage(string msg)
    {
        var match = Regex.Match(msg, @"^(\b[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}\b)\s+(\d+)$");
        if (!match.Success) throw new InvalidRatingSubmissionMessageException($"\"{msg}\" is an invalid rating submission message");

        var serviceProviderId = Guid.Parse(match.Groups[1].Value);
        var rating = uint.Parse(match.Groups[2].Value);
        var ratingSubmission = new RatingSubmission { ServiceProviderId = serviceProviderId, Rating = rating };
        return ratingSubmission;
    }
}

public class ServiceProviderNotFoundException : Exception
{
    public ServiceProviderNotFoundException(string message) : base(message) { }
}

public class InvalidRatingSubmissionMessageException : Exception
{
    public InvalidRatingSubmissionMessageException(string message) : base(message) { }
}