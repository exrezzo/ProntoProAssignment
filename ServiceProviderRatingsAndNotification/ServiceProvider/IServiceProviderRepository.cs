using System.Data.SqlClient;
using Dapper;

namespace ServiceProviderRatingsAndNotification.ServiceProvider;

public interface IServiceProviderRepository
{
    Task<IEnumerable<ServiceProvider>> GetAllAsync();
    Task<ServiceProvider> GetAsync(Guid serviceProviderId);
    Task<IEnumerable<int>> GetRatingsAsync(Guid serviceProviderId);
    Task AddRatingAsync(Guid serviceProviderId, uint rating);
}

public class ServiceProviderRepository : IServiceProviderRepository
{
    private readonly string _connectionString;

    public ServiceProviderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<ServiceProvider>> GetAllAsync()
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        var serviceProvider = await sqlConnection.QueryAsync<ServiceProvider>("SELECT * FROM ServiceProvider");
        return serviceProvider;
    }

    public async Task<ServiceProvider> GetAsync(Guid serviceProviderId)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        var serviceProvider = await sqlConnection.QuerySingleOrDefaultAsync<ServiceProvider>("SELECT * FROM ServiceProvider where Id = @serviceProviderId", new {serviceProviderId});
        return serviceProvider;
    }

    public async Task<IEnumerable<int>> GetRatingsAsync(Guid serviceProviderId)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        var ratings = await sqlConnection.QueryAsync<int>("SELECT Rating FROM ServiceProviderRating where ServiceProviderId = @serviceProviderId", new{serviceProviderId});
        return ratings;
    }

    public async Task AddRatingAsync(Guid serviceProviderId, uint rating)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await sqlConnection.ExecuteAsync(
            "INSERT INTO ServiceProviderRating(ServiceProviderId, Rating) VALUES (@serviceProviderId, @rating)",
            new { serviceProviderId, rating = (int) rating });
    }
}
