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

public partial class HistoryViewModel : INotifyPropertyChanged
{
    private ObservableCollection<PlayHistoryWithSongDTO> _songs = new ObservableCollection<PlayHistoryWithSongDTO>();
    public ObservableCollection<PlayHistoryWithSongDTO> Songs
    {
        get => _songs;
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }
    public HistoryViewModel(){}

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}