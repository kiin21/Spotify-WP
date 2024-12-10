// SearchResultPageViewModel.cs
using Microsoft.UI.Xaml.Controls;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.Views;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using Spotify.Contracts.Services;

namespace Spotify.ViewModels;

/// <summary>
/// ViewModel for managing search results and navigation to song details.
/// </summary>
public class SearchResultPageViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private ObservableCollection<SongDTO> _searchResults;

    /// <summary>
    /// Command executed when a song is selected.
    /// </summary>
    public ICommand SongSelectedCommand { get; private set; }

    /// <summary>
    /// Gets or sets the search results.
    /// </summary>
    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultPageViewModel"/> class.
    /// </summary>
    /// <param name="searchResults">The search results.</param>
    /// <param name="navigationService">The navigation service.</param>
    public SearchResultPageViewModel(
        ObservableCollection<SongDTO> searchResults,
        INavigationService navigationService)
    {
        SearchResults = searchResults;
        _navigationService = navigationService;

        SongSelectedCommand =
                        new RelayCommand(
                                execute: param => NavigateToSongDetail(param as SongDTO),
                                canExecute: param => param is SongDTO);
    }

    /// <summary>
    /// Navigates to the song detail page.
    /// </summary>
    /// <param name="song">The selected song.</param>
    private void NavigateToSongDetail(SongDTO song)
    {
        if (song != null)
        {
            _navigationService.Navigate(typeof(SongDetailPage), song);
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
