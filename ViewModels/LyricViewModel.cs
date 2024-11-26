//LyricViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spotify.Models.DTOs;

namespace Spotify.ViewModels;

public class LyricViewModel : INotifyPropertyChanged
{
    private SongDTO _song;
    public SongDTO Song
    {
        get => _song;
        private set
        {
            if (_song != value)
            {
                _song = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<string> _lyricLines = new ObservableCollection<string>(new[] { "No lyrics available" });
    public ObservableCollection<string> LyricLines
    {
        get => _lyricLines ?? _lyricLines;
        private set
        {
            if (_lyricLines != value)
            {
                _lyricLines = value;
                OnPropertyChanged();
            }
        }
    }

    public LyricViewModel(SongDTO song)
    {
        Song = song ?? throw new ArgumentNullException(nameof(song));
        _lyricLines = new ObservableCollection<string>();
    }

    public void LoadLyrics()
    {
        LyricLines.Clear();

        // If synced lyrics are available, use those
        if (!string.IsNullOrEmpty(Song.syncedLyrics))
        {
            foreach (var (_, text) in Song.ParsedSyncedLyrics)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    LyricLines.Add(text);
                }
            }
        }
        // Fall back to plain lyrics if available
        else if (!string.IsNullOrEmpty(Song.plainLyrics))
        {
            var lines = Song.plainLyrics.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    LyricLines.Add(line.Trim());
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}