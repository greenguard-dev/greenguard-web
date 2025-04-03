namespace web.Services.Plant;

public interface IPlantService
{
    Task<IReadOnlyList<Store.Plant>> GetPlantsAsync();
    Task<Store.Plant?> GetPlantAsync(Guid id);
    Task AddPlantAsync(Guid id, string? name);
    Task UpdatePlantAsync(Store.Plant plant);
    Task DeletePlantAsync(Guid id);
}