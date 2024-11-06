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

public class SearchResultPageViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;

    private ObservableCollection<SongDTO> _searchResults;

    public ICommand SongSelectedCommand { get; private set; }

    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

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

    private void NavigateToSongDetail(SongDTO song)
    {
        if (song != null)
        {
            _navigationService.Navigate(typeof(SongDetailPage), song);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
