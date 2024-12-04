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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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

    private readonly PlaylistService _playlistService;

    private ObservableCollection<PlaylistDTO> _playlists = new ObservableCollection<PlaylistDTO>();
    public ObservableCollection<PlaylistDTO> Playlists
    {
        get => _playlists;
        set
        {
            _playlists = value;
            OnPropertyChanged(nameof(Playlists));
        }
    }

    public SearchResultPageViewModel(
        ObservableCollection<SongDTO> searchResults,
        INavigationService navigationService,
        PlaylistService playlistService)
    {
        SearchResults = searchResults;
        _navigationService = navigationService;
        _playlistService = playlistService;

        SongSelectedCommand = new RelayCommand(
            execute: param => NavigateToSongDetail(param as SongDTO),
            canExecute: param => param is SongDTO);

        // Load Playlists
        LoadPlaylistsAsync();
    }

    private async Task LoadPlaylistsAsync()
   {
        try
        {
            var userId = (App.Current as App)?.CurrentUser?.Id;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("CurrentUser is null or userId is empty.");
                return;
            }

            var playlists = await _playlistService.GetPlaylistsByUserIdAsync(userId);

            if (playlists != null)
            {
                Playlists = new ObservableCollection<PlaylistDTO>(playlists);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading playlists: {ex.Message}");
        }
    }

    private void NavigateToSongDetail(SongDTO song)
    {
        if (song != null)
        {
            _navigationService.Navigate(typeof(SongDetailPage), song);
        }
    }
    public async Task AddSongToPlaylistAsync(SongDTO song, string playlistId)
    {
        if (song == null || string.IsNullOrEmpty(playlistId))
        {
            throw new ArgumentException("Song or Playlist ID is invalid.");
        }

        try
        {
            // Tạo đối tượng chi tiết bài hát
            var songDetail = new PlaylistSongDetailDTO
            {
                SongId = song.Id.ToString(),
                AddedBy = (App.Current as App)?.CurrentUser?.Username,
                Avatar = song.CoverArtUrl
            };

            // Gọi PlaylistSongDetailService để thêm bài hát vào Playlist
            var playlistSongService = App.Current.Services.GetService<PlaylistSongDetailService>();
            await playlistSongService.AddSongToPlaylistAsync(playlistId, songDetail);

            System.Diagnostics.Debug.WriteLine($"Song '{song.title}' added to playlist '{playlistId}'.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to playlist: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
