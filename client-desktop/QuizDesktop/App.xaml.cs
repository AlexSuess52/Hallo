using QuizDesktop.Services;

namespace QuizDesktop;
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        Application.Current?.Dispatcher.Dispatch(() =>
        {
            _ = NavigateOnStartupAsync();
        });
    }

    private async Task NavigateOnStartupAsync()
    {
        var token = await TokenService.LoadTokenAsync();

        if (!string.IsNullOrWhiteSpace(token) && !TokenService.IsTokenExpired(token))
        {
            // Optional: Token validieren
            await Shell.Current.GoToAsync("//main");
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}