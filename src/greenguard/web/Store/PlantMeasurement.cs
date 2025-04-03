namespace web.Store;

public class PlantMeasurement
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
    public Dictionary<string, int> Items { get; set; }
}