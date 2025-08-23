using CommunityToolkit.Mvvm.ComponentModel;
using QuizDesktop.Services;
using CommunityToolkit.Mvvm.Input;
namespace QuizDesktop.ViewModel;

public partial class RegisterViewModel : ObservableObject
{
    private readonly RegisterService _registerService = new();
    
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string? _email;
    
    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Fehler", "Benutzername und Passwort dürfen nicht leer sein.", "OK");
            return;
        }

        var result = await _registerService.RegisterUser(Username, Password, Email);

        if (result.Success)
        {
            await Shell.Current.DisplayAlert("Erfolg", result.Message, "OK");
            await Shell.Current.GoToAsync("//login");
        }
        else
        {
            await Shell.Current.DisplayAlert("Fehler", result.Message ?? "Unbekannter Fehler", "OK");
        }
    }
    
    [RelayCommand]
    public void BackToLogin()
    {
        Shell.Current.GoToAsync("//login");
    }
    

}