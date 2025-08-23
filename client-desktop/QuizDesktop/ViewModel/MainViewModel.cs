using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizDesktop;
using QuizDesktop.Models;
using QuizDesktop.Services;
using QuizDesktop.ViewModel;

namespace QuizDesktop.ViewModel
{
    public enum TableViewMode
    {
        None,
        Players,
        Leaderboard
    }
}

public partial class MainViewModel : ObservableObject
{

    private readonly PlayerService _playerService = new();
    
    [ObservableProperty]
    private TableViewMode _currentViewMode = TableViewMode.None;

    public bool ShowPlayersTable => CurrentViewMode == TableViewMode.Players;
    public bool ShowLeaderboardTable => CurrentViewMode == TableViewMode.Leaderboard;

    [ObservableProperty] private ObservableCollection<string> _items;

    [ObservableProperty] private string? _text;

    public ObservableCollection<PlayerResponse> Players { get; } = new();
    public ObservableCollection<LeaderboardResponse> Leaderboard { get; } = new();
    private readonly LeaderboardService _leaderBoardService = new();

    public MainViewModel()
    {
        Items = new ObservableCollection<string>();
    }
    
    partial void OnCurrentViewModeChanged(TableViewMode value)
    {
        Console.WriteLine($"viewmodel changed to: {value}");
        OnPropertyChanged(nameof(ShowPlayersTable));
        OnPropertyChanged(nameof(ShowLeaderboardTable));
    }

    [RelayCommand]
    private async Task GetAllPlayers()
    {
        try
        {
            var result = await _playerService.GetAllPlayersAsync();
            if (result != null)
            {
                Players.Clear();
                foreach (var player in result.OrderBy(p => p.Id))
                {
                    Players.Add(player);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error while loading the players: {ex.Message}");
        }
        finally
        {
            CurrentViewMode = TableViewMode.Players;
        }
    }
    
    [RelayCommand]
    private async Task ShowLeaderboard()
    {
        try
        {
            var recalcSuccess = await _leaderBoardService.RecalculateLeaderboardAsync();
            if (!recalcSuccess)
            {
                Console.WriteLine("error while recalcing the leaderboard");
                return;
            }

            var result = await _leaderBoardService.GetLeaderboardAsync();
            if (result != null)
            {
                Leaderboard.Clear();
                foreach (var entry in result.OrderBy(e => e.Rank))
                {
                    Leaderboard.Add(entry);
                }

                CurrentViewMode = TableViewMode.Leaderboard;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error while loading the leaderboard: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task Logout()
    {
        TokenService.RemoveTokens();
        Text = string.Empty;
        CurrentViewMode = TableViewMode.None;
        await Shell.Current.GoToAsync("//login");
    }

}