using Microsoft.UI.Xaml;

namespace QuizDesktop.WinUI;

/// <summary>
/// Stellt den Anwendungseinstiegspunkt bereit.
/// </summary>
public sealed partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}