namespace web.Store;

public abstract class PlantMeasurement
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
}