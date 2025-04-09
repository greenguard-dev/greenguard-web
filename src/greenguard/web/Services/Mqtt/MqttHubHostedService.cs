using System.Text.Json;
using MQTTnet;
using MQTTnet.Server;
using web.Services.Hub;

namespace web.Services.Mqtt;

public class MqttHubHostedService : IHostedService
{
    private readonly MqttServer _mqttServer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MqttHubHostedService(MqttServer mqttServer, IServiceScopeFactory serviceScopeFactory)
    {
        _mqttServer = mqttServer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _mqttServer.InterceptingPublishAsync += async e =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            var message = e.ApplicationMessage.ConvertPayloadToString();

            switch (e.ApplicationMessage.Topic)
            {
                case MqttConstants.HealthCheckTopic:
                    var healthCheckMessage = JsonSerializer.Deserialize<HealthCheckMessage>(message);
                    await hubService.HealthCheckAsync(Guid.Parse(e.ClientId), healthCheckMessage.IpAddress);
                    break;
                case MqttConstants.SensorDataTopic:
                    var sensorDataMessage = JsonSerializer.Deserialize<SensorDataMessage>(message);
                    break;
                case MqttConstants.ConfigurationUpdateTopic:
                    break;
            }
        };

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private class HealthCheckMessage
    {
        public required string IpAddress { get; set; }
    }

    private class SensorDataMessage
    {
        public required string Address { get; set; }
        public required string Data { get; set; }
    }
}