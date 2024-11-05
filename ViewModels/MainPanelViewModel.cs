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
public class MainPanelViewModel : INotifyPropertyChanged
{
    private SongService _songService;
    private ObservableCollection<SongDTO> _songs= new ObservableCollection<SongDTO>();
    public ObservableCollection<SongDTO> Songs
    {
        get => _songs;
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }
    public MainPanelViewModel(SongService songService)
    {
        _songService = songService;
        LoadAllSongs();
    }
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
    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}