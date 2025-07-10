using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using StateMachineMapper.Constants;
using Microsoft.Extensions.Options;

namespace StateMachineMapper.Manager;

public class RabbitMqManager
{
    private readonly HttpClient _httpClient;
    private readonly string _vhost;

    public RabbitMqManager(IOptions<AppSettings> options)
    {
        var appSettings = options.Value;

        _httpClient = new HttpClient { BaseAddress = new Uri($"http://{appSettings.RabbitMq.Host}:{appSettings.RabbitMq.Port}") };

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{appSettings.RabbitMq.Username}:{appSettings.RabbitMq.Password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        _vhost = appSettings.RabbitMq.VHost == "/" ? "%2f" : appSettings.RabbitMq.VHost;
    }

    public async Task DeleteEndpointTopology(string endpointName)
    {
        await DeleteQueue(endpointName);
        await DeleteExchange(endpointName);
    }

    private async Task DeleteQueue(string queueName)
    {
        await _httpClient.DeleteAsync($"/api/queues/{_vhost}/{queueName}");
    }

    private async Task DeleteExchange(string exchangeName)
    {
        await _httpClient.DeleteAsync($"/api/exchanges/{_vhost}/{exchangeName}");
    }
}
