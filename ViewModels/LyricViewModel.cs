//LyricViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spotify.Models.DTOs;
using Spotify.Helpers;
using System.Diagnostics;
using Windows.Networking.Sockets;
using System.Linq;

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
    private readonly PlaybackControlViewModel _playbackViewModel;

    private ObservableCollection<LyricLine> _lyricLines = new ObservableCollection<LyricLine>();
    public ObservableCollection<LyricLine> LyricLines
    {
        get => _lyricLines;
        private set
        {
            if (_lyricLines != value)
            {
                _lyricLines = value;
                OnPropertyChanged();
            }
        }
    }
    public LyricLine HighlightedLyric => LyricLines.FirstOrDefault(line => line.IsHighlighted);
    //public void ScrollToHighlightedLyric()
    //{
    //    var highlightedLyric = HighlightedLyric;
    //    if (highlightedLyric != null)
    //    {
    //        HighlightedLyricChanged?.Invoke(this, highlightedLyric);
    //    }
    //}

    public event EventHandler<LyricLine> HighlightedLyricChanged;
    public LyricViewModel(SongDTO song)
    {
        Song = song;
        _playbackViewModel = PlaybackControlViewModel.Instance;
        _playbackViewModel.PropertyChanged += PlaybackViewModel_PropertyChanged;
    }
    private void PlaybackViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlaybackControlViewModel.CurrentPositionSeconds))
        {
            TimeSpan current_time = TimeSpan.FromSeconds(_playbackViewModel.CurrentPositionSeconds);
            List<(TimeSpan Timestamp, string Text)> map = Song.ParsedSyncedLyrics;

            for (int i = 0; i < map.Count; i++)
            {
                if (current_time < map[i].Timestamp)
                {
                    // Update highlighted states
                    for (int j = 0; j < LyricLines.Count; j++)
                    {
                        LyricLines[j].IsHighlighted = (j == i - 1);
                        if (LyricLines[j].IsHighlighted)
                        {
                            HighlightedLyricChanged?.Invoke(this, LyricLines[j]);
                        }
                    }
                    break;
                }
            }
        }
        else if (e.PropertyName == nameof(PlaybackControlViewModel.CurrentSong))
        {
            Song = _playbackViewModel.CurrentSong;
            LoadLyrics();
        }
    }

    public void LoadLyrics()
    {
        LyricLines.Clear();

        if (!string.IsNullOrEmpty(Song.syncedLyrics))
        {
            foreach (var (_, text) in Song.ParsedSyncedLyrics)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Modify latter part of this project
                    LyricLines.Add(new LyricLine(text, false));
                }
            }
        }
        else if (!string.IsNullOrEmpty(Song.plainLyrics))
        {
            var lines = Song.plainLyrics.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {

                    // Modify latter part of this project
                    LyricLines.Add(new LyricLine(line.Trim(), false));
                }
            }
        }
        else
        {
            LyricLines = new ObservableCollection<LyricLine>(
                new List<LyricLine>
                {
                    new LyricLine("No lyrics available", false)
                }
            );
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


