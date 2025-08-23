using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using QuizDesktop.Models;

namespace QuizDesktop.Services;

public class LoginService
{
    private readonly HttpClient _httpClient = new();
    string _aspnetBackendUrl = "http://localhost:5200/";

    public async Task<LoginResponse> LoginUser(string username, string password)
    {
        var url = _aspnetBackendUrl + "api/auth/login";
        var playerObject = new { name = username, password = password };
        var json = JsonSerializer.Serialize(playerObject);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (result != null)
                {
                    await TokenService.SaveTokenAsync(result!.AccessToken);
                    return new LoginResponse
                    {
                        Token = result,
                        Success = true
                    };
                }
                return new LoginResponse
                {
                    Success = false,
                    Message = "Token konnte nicht gelesen werden."
                };
            }
            else
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Login fehlgeschlagen. Bitte überprüfe deine Eingaben."
                };
            }
        }
        catch (HttpRequestException)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Server nicht erreichbar."
            };
        }
        catch (TaskCanceledException)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Timeout – der Server hat nicht geantwortet."
            };
        }
        catch (Exception)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Unerwarteter Fehler beim Login."
            };
        }
    }


}