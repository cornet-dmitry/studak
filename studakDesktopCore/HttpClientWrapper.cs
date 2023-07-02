using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace studakDesktopCore;

public class HttpClientWrapper : IDisposable
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper()
    {
        _httpClient = new HttpClient();
    }

    public async Task<HttpResponseMessage> GetAsync(string method)
    {
        return await _httpClient.GetAsync("https://localhost:7156/api/" + method);
    }

    public async Task<HttpResponseMessage> PostAsync(string method, HttpContent content)
    {
        return await _httpClient.PostAsync("https://localhost:7156/api/" + method, content);
    }

    // Другие методы для работы с HttpClient

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}