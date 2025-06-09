using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Comfort.Models;

namespace Comfort.Services;

public interface IDatabaseService
{
    Task<bool> TestConnectionAsync();
    ApplicationDbContext CreateDbContext();
}

public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly IErrorHandlingService _errorHandling;
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IErrorHandlingService errorHandling)
    {
        _logger = logger;
        _errorHandling = errorHandling;
        _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var context = CreateDbContext();
            return await context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            _errorHandling.LogError(ex, "Ошибка при тестировании подключения к БД");
            return false;
        }
    }

    public ApplicationDbContext CreateDbContext()
    {
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
        catch (Exception ex)
        {
            _errorHandling.LogError(ex, "Ошибка при создании контекста базы данных");
            throw;
        }
    }
} 