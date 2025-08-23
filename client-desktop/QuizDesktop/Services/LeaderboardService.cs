using System.Net.Http.Json;
using QuizDesktop.Models;

namespace QuizDesktop.Services;

public class LeaderboardService
{
    private readonly HttpClient _httpClient;
    private readonly string _aspnetBackendUrl = "http://localhost:5200/";

    public LeaderboardService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<LeaderboardResponse>?> GetLeaderboardAsync()
    {
        var url = _aspnetBackendUrl + "api/leaderboard/get-current-ranking";
        var token = await RequestService.GetTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<LeaderboardResponse>>(url);
            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP-error leaderboard: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"general error leaderboard: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RecalculateLeaderboardAsync()
    {
        var url = _aspnetBackendUrl + "api/leaderboard/recalculate-leaderboard";
        var token = await RequestService.GetTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP-error recalculate: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"general error recalculate: {ex.Message}");
            return false;
        }
    }
}