namespace QuizDesktop.Models;

public class LoginResponse
{
    public TokenResponse? Token { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
