namespace QuizDesktop.Models;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public TokenResponse? Token { get; set; }
}