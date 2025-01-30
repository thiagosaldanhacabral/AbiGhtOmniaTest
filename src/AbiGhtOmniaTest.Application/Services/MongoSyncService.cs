using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AbiGhtOmniaTest.Application.Services;

public class MongoSyncService(IServiceProvider serviceProvider, ILogger<MongoSyncService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<MongoSyncService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Starting sync between PostgreSQL and MongoDB...");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DeveloperStoreDbContext>();
            var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
            var salesCollection = mongoClient.GetDatabase("DeveloperStore").GetCollection<Sale>("Sales");
            var usersCollection = mongoClient.GetDatabase("DeveloperStore").GetCollection<User>("Users");

            var sales = await dbContext.Sales.Include(s => s.Items).ToListAsync(stoppingToken);
            var users = await dbContext.Users.ToListAsync(stoppingToken);

            foreach (var sale in sales)
            {
                await salesCollection.ReplaceOneAsync(
                    s => s.SaleId == sale.SaleId,
                    sale,
                    new ReplaceOptions { IsUpsert = true },
                    stoppingToken);
            }

            foreach (var user in users)
            {
                await usersCollection.ReplaceOneAsync(
                    u => u.Id == user.Id,
                    user,
                    new ReplaceOptions { IsUpsert = true },
                    stoppingToken);
            }

            _logger.LogInformation("Sync completed successfully.");
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}