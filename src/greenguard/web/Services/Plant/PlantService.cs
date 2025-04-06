using Marten;
using web.Store;

namespace web.Services.Plant;

public class PlantService : IPlantService
{
    private readonly IDocumentSession _documentSession;

    public PlantService(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public Task<IReadOnlyList<Store.Plant>> GetPlantsAsync()
    {
        return _documentSession.Query<Store.Plant>().ToListAsync();
    }

    public Task<Store.Plant?> GetPlantAsync(Guid id)
    {
        return _documentSession.LoadAsync<Store.Plant>(id);
    }

    public Task AddManuelPlantAsync(Guid id, string? name, int wateringInterval, int fertilizingInterval)
    {
        var plant = new ManuelPlant
        {
            Id = id,
            Name = name,
            WateringInterval = wateringInterval,
            FertilizingInterval = fertilizingInterval
        };

        _documentSession.Store(plant);
        return _documentSession.SaveChangesAsync();
    }

    public Task AddSensorPlantAsync(Guid id, string name, string address)
    {
        var plant = new SensorPlant
        {
            Id = id,
            Name = name,
            Address = address
        };

        _documentSession.Store(plant);
        return _documentSession.SaveChangesAsync();
    }

    public Task UpdatePlantAsync(Store.Plant plant)
    {
        _documentSession.Update(plant);
        return _documentSession.SaveChangesAsync();
    }

    public Task DeletePlantAsync(Guid id)
    {
        _documentSession.Delete<Store.Plant>(id);
        return _documentSession.SaveChangesAsync();
    }
}