using System.IdentityModel.Tokens.Jwt;

namespace QuizDesktop.Services;
//  rm /Users/schoeni/Library/Containers/ch.oop-projekt.quizdesktop/Data/Documents/.config/QuizDesktop/token.txt
public class TokenService
{
    private static string AppDataPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "QuizDesktop");

    private static string AccessTokenPath => Path.Combine(AppDataPath, "token.txt");
    private static string RefreshTokenPath => Path.Combine(AppDataPath, "refresh_token.txt");

    public static async Task SaveTokenAsync(string token)
    {
        Directory.CreateDirectory(AppDataPath);
        await File.WriteAllTextAsync(AccessTokenPath, token);
    }

    public static async Task<string?> LoadTokenAsync()
    {
        return File.Exists(AccessTokenPath)
            ? await File.ReadAllTextAsync(AccessTokenPath)
            : null;
    }

    public static async Task SaveRefreshTokenAsync(string refreshToken)
    {
        Directory.CreateDirectory(AppDataPath);
        await File.WriteAllTextAsync(RefreshTokenPath, refreshToken);
    }

    public static async Task<string?> LoadRefreshTokenAsync()
    {
        return File.Exists(RefreshTokenPath)
            ? await File.ReadAllTextAsync(RefreshTokenPath)
            : null;
    }

    public static bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        if (expClaim == null) return true;

        var expUnix = long.Parse(expClaim.Value);
        var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        return DateTime.UtcNow > expDateTime;
    }

    public static void RemoveTokens()
    {
        if (File.Exists(AccessTokenPath))
            File.Delete(AccessTokenPath);

        if (File.Exists(RefreshTokenPath))
            File.Delete(RefreshTokenPath);
    }
}