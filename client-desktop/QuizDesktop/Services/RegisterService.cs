using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using QuizDesktop.Models;

namespace QuizDesktop.Services;

public class RegisterService
{
    private readonly HttpClient _httpClient = new();
    private readonly string _aspnetBackendUrl = "http://localhost:5200/";

    public async Task<RegisterResponse> RegisterUser(string username, string password, string? email = null)
    {
        var url = _aspnetBackendUrl + "api/auth/register";
        var playerObject = new { name = username, password = password };
        var json = JsonSerializer.Serialize(playerObject);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponse>();

                if (tokenResult != null)
                {
                    await TokenService.SaveTokenAsync(tokenResult!.AccessToken);
                    await TokenService.SaveRefreshTokenAsync(tokenResult!.RefreshToken);

                    return new RegisterResponse
                    {
                        Success = true,
                        Message = "Registrierung erfolgreich.",
                        Token = tokenResult
                    };
                }

                return new RegisterResponse
                {
                    Success = false,
                    Message = "Token konnte nicht gelesen werden."
                };
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Fehler: {response.StatusCode} – {body}"
                };
            }
        }
        catch (Exception ex)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = $"Netzwerkfehler: {ex.Message}"
            };
        }
    }
}
