using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spotify.Models.DTOs;

namespace Spotify.ViewModels
{
    /// <summary>
    /// ViewModel for managing play history and grouping songs.
    /// </summary>
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PlayHistoryWithSongDTO> _songs = new ObservableCollection<PlayHistoryWithSongDTO>();

        /// <summary>
        /// Gets or sets the collection of play history songs.
        /// </summary>
        public ObservableCollection<PlayHistoryWithSongDTO> Songs
        {
            get => _songs;
            set
            {
                _songs = value;
                OnPropertyChanged();
                LoadAndGroupSongs();
            }
        }

        private ObservableCollection<GroupedPlayHistory> _groupedSongs = new ObservableCollection<GroupedPlayHistory>();

        /// <summary>
        /// Gets or sets the grouped collection of play history songs.
        /// </summary>
        public ObservableCollection<GroupedPlayHistory> GroupedSongs
        {
            get => _groupedSongs;
            set
            {
                _groupedSongs = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// </summary>
        public HistoryViewModel()
        {
            // Initialization logic if needed
        }

        private void LoadAndGroupSongs()
        {
            var grouped = Songs
                .GroupBy(song => GetGroupKey(song.PlayedAt))
                .Select(g => new GroupedPlayHistory(g.Key, new ObservableCollection<PlayHistoryWithSongDTO>(g)))
                .OrderByDescending(g => g.Key)
                .ToList();

            GroupedSongs = new ObservableCollection<GroupedPlayHistory>(grouped);
        }

        private string GetGroupKey(DateTime date)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            if (date.Date == today)
                return "Today";
            if (date.Date >= startOfWeek && date.Date < startOfWeek.AddDays(7))
                return "This Week";
            if (date.Date >= startOfWeek.AddDays(-7) && date.Date < startOfWeek)
                return "Last Week";
            return date.ToString("MMM-yyyy");
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

    /// <summary>
    /// Represents a grouped collection of play history songs.
    /// </summary>
    public class GroupedPlayHistory : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key for the group.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the collection of play history songs in the group.
        /// </summary>
        public ObservableCollection<PlayHistoryWithSongDTO> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedPlayHistory"/> class.
        /// </summary>
        /// <param name="key">The key for the group.</param>
        /// <param name="items">The collection of play history songs in the group.</param>
        public GroupedPlayHistory(string key, ObservableCollection<PlayHistoryWithSongDTO> items)
        {
            Key = key;
            Items = items;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
