using System.Data.SqlClient;
using Dapper;

namespace ServiceProviderRatingsAndNotification.Rating;

public interface IServiceProviderRepository
{
    Task<ServiceProvider.ServiceProvider> GetAsync(Guid serviceProviderId);
    Task<IEnumerable<int>> GetRatingsAsync(Guid serviceProviderId);
    Task AddRatingAsync(Guid serviceProviderId, int rating);
}

public class ServiceProviderRepository : IServiceProviderRepository
{
    private readonly string _connectionString;

    public ServiceProviderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<ServiceProvider.ServiceProvider> GetAsync(Guid serviceProviderId)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        var serviceProvider = await sqlConnection.QuerySingleOrDefaultAsync<ServiceProvider.ServiceProvider>("SELECT * FROM ServiceProvider where Id = @serviceProviderId", new {serviceProviderId});
        return serviceProvider;
    }

    public async Task<IEnumerable<int>> GetRatingsAsync(Guid serviceProviderId)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        var ratings = await sqlConnection.QueryAsync<int>("SELECT Rating FROM ServiceProviderRating where ServiceProviderId = @serviceProviderId", new{serviceProviderId});
        return ratings;
    }

    public Task AddRatingAsync(Guid serviceProviderId, int rating)
    {
        throw new NotImplementedException();
    }
}
