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

/// <summary>
/// ViewModel for managing lyrics display and synchronization with playback.
/// </summary>
public class LyricViewModel : INotifyPropertyChanged
{
    private SongDTO _song;
    /// <summary>
    /// Gets or sets the current song.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the collection of lyric lines.
    /// </summary>
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
    /// <summary>
    /// Gets the currently highlighted lyric line.
    /// </summary>
    public LyricLine HighlightedLyric => LyricLines.FirstOrDefault(line => line.IsHighlighted);

    /// <summary>
    /// Occurs when the highlighted lyric line changes.
    /// </summary>
    public event EventHandler<LyricLine> HighlightedLyricChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricViewModel"/> class.
    /// </summary>
    /// <param name="song">The song for which to display lyrics.</param>
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

    /// <summary>
    /// Loads the lyrics for the current song.
    /// </summary>
    public void LoadLyrics()
    {
        LyricLines.Clear();

        if (!string.IsNullOrEmpty(Song.syncedLyrics))
        {
            foreach (var (_, text) in Song.ParsedSyncedLyrics)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
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
