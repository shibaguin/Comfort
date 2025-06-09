using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comfort.Services
{
    // Интерфейс сервиса для работы с цехами производства
    public interface IWorkshopService
    {
        // Получение списка цехов, связанных с продуктом
        Task<List<WorkshopInfo>> GetWorkshopsForProduct(int productId);
        
        // Расчет необходимого количества сырья для производства
        Task<int> CalculateRawMaterials(int productTypeId, int materialTypeId, int quantity, double param1, double param2);
    }

    // Информация о цехе производства
    public class WorkshopInfo
    {
        // Название цеха
        public required string WorkshopName { get; set; }
        // Количество сотрудников в цехе
        public int StaffCount { get; set; }
        // Время производства в часах
        public decimal ManufacturingTime { get; set; }
    }
} 