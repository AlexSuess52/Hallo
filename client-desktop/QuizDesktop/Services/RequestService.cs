namespace QuizDesktop.Services;

public class RequestService
{
    public static async Task<string?> GetTokenAsync()
    {
        var token = await TokenService.LoadTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("no valid token found");
            return null;
        }
        return token;
    }
}