using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// ViewModel for managing playlist pages, including loading and playing playlists.
/// </summary>
public class PlaylistPageViewModel : INotifyPropertyChanged
{
    private readonly PlaylistService _playlistService;
    private readonly PlaylistSongDetailService _playlistSongDetailService;
    private ObservableCollection<PlaylistDTO> _playlists;
    private ObservableCollection<PlaylistSongDetailDTO> _playlistSongs;
    private PlaylistDTO _selectedPlaylist;

    /// <summary>
    /// Gets the command to play the selected playlist.
    /// </summary>
    public IRelayCommand PlaySelectedPlaylistCommand { get; }

    /// <summary>
    /// Gets or sets the collection of playlists.
    /// </summary>
    public ObservableCollection<PlaylistDTO> Playlists
    {
        get => _playlists;
        set
        {
            _playlists = value;
            OnPropertyChanged(nameof(Playlists));
        }
    }

    /// <summary>
    /// Gets or sets the collection of songs in the selected playlist.
    /// </summary>
    public ObservableCollection<PlaylistSongDetailDTO> PlaylistSongs
    {
        get => _playlistSongs;
        set
        {
            _playlistSongs = value;
            OnPropertyChanged(nameof(PlaylistSongs));
            OnPropertyChanged(nameof(SongCount)); // Notify when Songs is updated
        }
    }

    /// <summary>
    /// Gets the count of songs in the selected playlist.
    /// </summary>
    public string SongCount => $"{PlaylistSongs?.Count ?? 0} bài hát";

    /// <summary>
    /// Gets or sets the selected playlist.
    /// </summary>
    public PlaylistDTO SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            if (_selectedPlaylist != value)
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
                SelectedPlaylistIdChanged?.Invoke(this, _selectedPlaylist?.Id.ToString() ?? string.Empty);
                _ = LoadPlaylistSongs(_selectedPlaylist?.Id);
            }
        }
    }

    /// <summary>
    /// Occurs when the selected playlist ID changes.
    /// </summary>
    public event EventHandler<string> SelectedPlaylistIdChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistPageViewModel"/> class.
    /// </summary>
    /// <param name="playlistService">The service for managing playlists.</param>
    /// <param name="playlistSongDetailService">The service for managing playlist song details.</param>
    public PlaylistPageViewModel(PlaylistService playlistService, PlaylistSongDetailService playlistSongDetailService)
    {
        _playlistService = playlistService;
        _playlistSongDetailService = playlistSongDetailService;
        _playlists = new ObservableCollection<PlaylistDTO>();
        _playlistSongs = new ObservableCollection<PlaylistSongDetailDTO>();

        PlaySelectedPlaylistCommand = new RelayCommand(TogglePlay_Playlist);
    }

    private async void TogglePlay_Playlist()
    {
        List<string> song_ids = PlaylistSongs.Select(ps => ps.SongId).ToList();
        SongService songService = App.Current.Services.GetService<SongService>();
        List<SongDTO> songs = new List<SongDTO>();
        foreach (var id in song_ids)
        {
            SongDTO song = await songService.GetSongByIdAsync(id);
            songs.Add(song);
        }

        QueueService queueService = QueueService.GetInstance(
                                                            App.Current.Services.GetRequiredService<IQueueDAO>(),
                                                            App.Current.Services.GetRequiredService<ISongDAO>(),
                                                            App.Current.CurrentUser);

        try
        {
            await queueService.UpdateQueueAsync(App.Current.CurrentUser.Id, song_ids);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Loads the songs for the specified playlist asynchronously.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task LoadPlaylistSongs(string playlistId)
    {
        if (!string.IsNullOrEmpty(playlistId))
        {
            var songs = await _playlistSongDetailService.GetPlaylistSongDetailAsync(playlistId);

            for (int i = 0; i < songs.Count; i++)
            {
                songs[i].Index = i + 1; // Bắt đầu từ 1

                // Kiểm tra xem bài hát có thuộc playlist "Liked Songs" không
                songs[i].IsInLikedPlaylist = SelectedPlaylist?.IsLikedSong ?? false;
            }

            PlaylistSongs = new ObservableCollection<PlaylistSongDetailDTO>(songs);
        }
        else
        {
            PlaylistSongs.Clear();
        }
    }

    /// <summary>
    /// Clears the selected playlist and its songs.
    /// </summary>
    public void ClearSelectedPlaylist()
    {
        SelectedPlaylist = null;
        PlaylistSongs.Clear();
    }

    /// <summary>
    /// Loads the selected playlist by its ID asynchronously.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist to load.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task LoadSelectedPlaylist(string playlistId)
    {
        if (SelectedPlaylist != null && SelectedPlaylist.Id == playlistId)
            return;

        ClearSelectedPlaylist();

        var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
        if (playlist != null)
        {
            SelectedPlaylist = playlist;
            await LoadPlaylistSongs(playlistId);
        }
    }

    /// <summary>
    /// Adds a new playlist asynchronously.
    /// </summary>
    /// <param name="playlistName">The name of the new playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddPlaylistAsync(string playlistName)
    {
        var newPlaylist = new PlaylistDTO
        {
            Title = playlistName,
            CreatedBy = (App.Current as App).CurrentUser.Username,
            CreatedAt = DateTime.Now,
            IsLikedSong = false,
            IsDeleted = false,
            Avatar = "https://example.com/default-cover.jpg"
        };

        await _playlistService.AddPlaylistAsync(newPlaylist);
        Playlists.Add(newPlaylist);
    }

    /// <summary>
    /// Removes the selected playlist asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RemoveSelectedPlaylist()
    {
        if (SelectedPlaylist != null)
        {
            // Mark the playlist as deleted in the database
            await _playlistService.UpdatePlaylistStatusAsync(SelectedPlaylist.Id, true);

            // Remove the playlist from local collection
            Playlists.Remove(SelectedPlaylist);

            // Clear the currently selected playlist
            ClearSelectedPlaylist();
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
