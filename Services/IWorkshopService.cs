using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comfort.Services
{
    public interface IWorkshopService
    {
        Task<List<WorkshopInfo>> GetWorkshopsForProduct(int productId);
        Task<int> CalculateRawMaterials(int productTypeId, int materialTypeId, int quantity, double param1, double param2);
    }

    public class WorkshopInfo
    {
        public required string WorkshopName { get; set; }
        public int StaffCount { get; set; }
        public decimal ManufacturingTime { get; set; }
    }
} 