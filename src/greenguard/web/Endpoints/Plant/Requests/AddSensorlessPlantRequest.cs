namespace web.Endpoints.Plant.Requests;

public class AddSensorlessPlantRequest
{
    public required string Name { get; set; }
    public string? WateringInterval { get; set; }
    public string? FertilizingInterval { get; set; }
}