using System.Net.Http.Json;
using QuizDesktop.Models;

namespace QuizDesktop.Services;

public class PlayerService
{
    private readonly HttpClient _httpClient;
    string _aspnetBackendUrl = "http://localhost:5200/";
    
    public PlayerService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<PlayerResponse>?> GetAllPlayersAsync()
    {
        var url = _aspnetBackendUrl + "api/auth/get-all-players";

        var token = await RequestService.GetTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PlayerResponse>>(url);
            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP-error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"general error: {ex.Message}");
            return null;
        }
    }
    
}