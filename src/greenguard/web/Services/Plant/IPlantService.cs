namespace web.Services.Plant;

public interface IPlantService
{
    Task<IReadOnlyList<Store.Plant>> GetPlantsAsync();
    Task<Store.Plant?> GetPlantAsync(Guid id);
    Task AddManuelPlantAsync(Guid id, string? name, int wateringInterval, int fertilizingInterval);
    Task AddSensorPlantAsync(Guid id, string name, string address);
    Task UpdatePlantAsync(Store.Plant plant);
    Task DeletePlantAsync(Guid id);
}