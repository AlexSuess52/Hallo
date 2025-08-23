using CommunityToolkit.Mvvm.ComponentModel;
using QuizDesktop.Services;
using CommunityToolkit.Mvvm.Input;

namespace QuizDesktop.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly LoginService _loginService = new();

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Fehler", "Benutzername und Passwort dürfen nicht leer sein.", "OK");
                return;
            }

            var result = await _loginService.LoginUser(Username, Password);
            
            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Erfolg", result.Message, "OK");
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await Shell.Current.DisplayAlert("Fehler", result.Message ?? "Unbekannter Fehler", "OK");
            }
        }
        
        [RelayCommand]
        public void RegisterRedirect()
        {
            Shell.Current.GoToAsync("//register");
        }
    }
}