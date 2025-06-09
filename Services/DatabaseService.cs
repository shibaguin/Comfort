using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Comfort.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comfort.Services;

// Интерфейс сервиса для работы с базой данных
public interface IDatabaseService
{
    // Проверка подключения к базе данных
    Task<bool> TestConnectionAsync();
    // Создание контекста базы данных
    ApplicationDbContext CreateDbContext();
    // Выполнение SQL-запроса с параметрами
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters);
}

// Реализация сервиса для работы с базой данных
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

    // Проверка возможности подключения к базе данных
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

    // Создание и настройка контекста базы данных
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

    // Выполнение SQL-запроса с параметрами и возврат результатов в виде DataTable
    public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters)
    {
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            using var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection);
            
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            var dataTable = new DataTable();
            using var adapter = new Microsoft.Data.SqlClient.SqlDataAdapter(command);
            await Task.Run(() => adapter.Fill(dataTable));
            
            return dataTable;
        }
        catch (Exception ex)
        {
            _errorHandling.LogError(ex, "Ошибка при выполнении запроса к БД");
            throw;
        }
    }
} 