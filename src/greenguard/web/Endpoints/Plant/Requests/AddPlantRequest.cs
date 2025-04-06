namespace web.Endpoints.Plant.Requests;

public class AddPlantRequest
{
    public string ManuallyName { get; set; }
    public int WateringInterval { get; set; }
    public int FertilizingInterval { get; set; }
}