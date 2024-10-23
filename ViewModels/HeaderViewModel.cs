// HeaderViewModel.cs
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.Views;

namespace Spotify.ViewModels;

public class HeaderViewModel : INotifyPropertyChanged
{
    private string _searchQuery;
    private readonly SongService _songService;
    private ObservableCollection<SongDTO> _searchResults;

    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged(nameof(SearchQuery));
        }
    }

    public ICommand SearchCommand { get; }

    public HeaderViewModel(SongService songService)
    {
        SearchCommand = new RelayCommand(async _ => await ExecuteSearchAsync(), _ => CanExecuteSearch());
        _songService = songService;
        SearchResults = new ObservableCollection<SongDTO>();
    }


    private async Task ExecuteSearchAsync()
    {
        var results = await _songService.SearchSongs(SearchQuery);
        SearchResults = new ObservableCollection<SongDTO>(results);

        var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
        var mainFrame = shellWindow.getMainFrame();
        shellWindow.NavigateToPage(typeof(SearchResultsPage), mainFrame, SearchResults);
    }
    // GetSuggestions method for auto-suggest functionality
    public async Task<List<string>> GetSuggestions(string query)
    {
        // Retrieve the suggestions based on the current query
        List<SongDTO> allSongs = await _songService.GetAllSongs(); // Assume you have a method to get all songs
        var suggestions = new List<string>();

        foreach (var song in allSongs)
        {
            if (song.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                song.Artist.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add(song.Title); // Add matching song titles
            }
        }

        return suggestions;
    }

    private bool CanExecuteSearch()
    {
        return !string.IsNullOrEmpty(SearchQuery);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
