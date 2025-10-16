using System.Net.Http.Json;
using Newtonsoft.Json;

namespace TestTaskKGR.ApiClient;

public abstract class BaseClient
{
    HttpClient _httpClient;
    public BaseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    protected async Task SendAsync(HttpMethod method, string endpoint)
    {
        using var request = new HttpRequestMessage(method, endpoint);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }
    }
    protected async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string endpoint, object? content = null)
    {
        using var request = new HttpRequestMessage(method, endpoint);

        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }
        return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());

    }

    protected Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
    {
        return SendAsync<TResponse>(HttpMethod.Post, endpoint, request);
    }

    protected Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        return SendAsync<TResponse>(HttpMethod.Get, endpoint);
    }

    protected Task DeleteAsync(string endpoint)
    {
        return SendAsync(HttpMethod.Delete, endpoint);
    }
}
