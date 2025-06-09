using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Comfort.Services
{
    // Реализация сервиса для работы с цехами производства
    public class WorkshopService : IWorkshopService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger _logger;

        public WorkshopService(IDatabaseService databaseService, ILogger logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        // Получение списка цехов, связанных с продуктом, из базы данных
        public async Task<List<WorkshopInfo>> GetWorkshopsForProduct(int productId)
        {
            try
            {
                _logger.Information("Getting workshops for product {ProductId}", productId);

                var query = @"
                    SELECT w.WorkshopName, w.StaffCount, pw.ManufacturingTime
                    FROM Workshops w
                    INNER JOIN ProductWorkshops pw ON w.WorkshopID = pw.WorkshopID
                    WHERE pw.ProductID = @ProductId
                    ORDER BY w.WorkshopName";

                var parameters = new Dictionary<string, object>
                {
                    { "@ProductId", productId }
                };

                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                return result.AsEnumerable()
                    .Select(row => new WorkshopInfo
                    {
                        WorkshopName = row.Field<string>("WorkshopName") ?? string.Empty,
                        StaffCount = row.Field<int>("StaffCount"),
                        ManufacturingTime = row.Field<decimal>("ManufacturingTime")
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting workshops for product {ProductId}", productId);
                throw;
            }
        }

        // Расчет необходимого количества сырья с учетом коэффициентов и потерь
        public async Task<int> CalculateRawMaterials(int productTypeId, int materialTypeId, int quantity, double param1, double param2)
        {
            try
            {
                _logger.Information("Calculating raw materials for product type {ProductTypeId}, material type {MaterialTypeId}, quantity {Quantity}", 
                    productTypeId, materialTypeId, quantity);

                if (param1 <= 0 || param2 <= 0 || quantity <= 0)
                {
                    _logger.Warning("Invalid parameters: param1={Param1}, param2={Param2}, quantity={Quantity}", 
                        param1, param2, quantity);
                    return -1;
                }

                var query = @"
                    SELECT pt.Coefficient, m.LossPercentage
                    FROM ProductTypes pt
                    INNER JOIN Materials m ON m.MaterialID = @MaterialTypeId
                    WHERE pt.ProductTypeID = @ProductTypeId";

                var parameters = new Dictionary<string, object>
                {
                    { "@ProductTypeId", productTypeId },
                    { "@MaterialTypeId", materialTypeId }
                };

                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                if (result.Rows.Count == 0)
                {
                    _logger.Warning("Product type {ProductTypeId} or material type {MaterialTypeId} not found", 
                        productTypeId, materialTypeId);
                    return -1;
                }

                var row = result.Rows[0];
                var coefficient = Convert.ToDecimal(row["Coefficient"]);
                var lossPercentage = Convert.ToDecimal(row["LossPercentage"]);

                decimal param1Decimal = (decimal)param1;
                decimal param2Decimal = (decimal)param2;

                // Расчет количества сырья с учетом коэффициентов и потерь
                var rawMaterialsPerUnit = param1Decimal * param2Decimal * coefficient;
                var totalRawMaterials = rawMaterialsPerUnit * quantity;
                var rawMaterialsWithLoss = totalRawMaterials * (1m + lossPercentage / 100m);

                _logger.Information("Calculated raw materials: {RawMaterials}", Math.Ceiling(rawMaterialsWithLoss));
                return (int)Math.Ceiling(rawMaterialsWithLoss);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error calculating raw materials");
                throw;
            }
        }
    }
} 