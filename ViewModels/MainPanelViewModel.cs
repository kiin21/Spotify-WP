using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spotify.Models;
using Spotify.Models.DTOs;
using Spotify.Services;

namespace Spotify.ViewModels;

/// <summary>
/// ViewModel for managing the main panel, including loading and displaying songs.
/// </summary>
public class MainPanelViewModel : INotifyPropertyChanged
{
    private SongService _songService;
    private ObservableCollection<SongDTO> _songs = new ObservableCollection<SongDTO>();

    /// <summary>
    /// Gets or sets the collection of songs.
    /// </summary>
    public ObservableCollection<SongDTO> Songs
    {
        get => _songs;
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPanelViewModel"/> class.
    /// </summary>
    /// <param name="songService">The service for managing song data.</param>
    public MainPanelViewModel(SongService songService)
    {
        _songService = songService;
        LoadAllSongs();
    }

    /// <summary>
    /// Loads all songs asynchronously and updates the Songs collection.
    /// </summary>
    public async void LoadAllSongs()
    {
        try
        {
            var allSongs = await _songService.GetAllSongs();
            Songs = new ObservableCollection<SongDTO>(allSongs);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., show a message to the user)
            Console.WriteLine($"Error loading songs: {ex.Message}");
        }
    }

    public void AddToQueueCommand(SongDTO song)
    {
        var playbackViewModel = PlaybackControlViewModel.Instance;
        var collection  = new ObservableCollection<SongDTO>();
        collection.Add(song);
        playbackViewModel.AddToPlaybackList(collection, false);
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
