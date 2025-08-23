using QuizDesktop.Services;
using QuizDesktop.ViewModel;

namespace QuizDesktop;

public partial class MainPage : ContentPage
{

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}