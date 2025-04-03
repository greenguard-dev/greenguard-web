using Marten;

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

    public Task AddPlantAsync(Guid id, string? name)
    {
        var plant = new Store.Plant
        {
            Id = id,
            Name = name
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