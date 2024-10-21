using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spotify.Models;

namespace Spotify.ViewModels;
public class MainPanelViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Song> _songs;
    public ObservableCollection<Song> Songs
    {
        get => _songs;
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}