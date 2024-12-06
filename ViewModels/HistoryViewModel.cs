using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spotify.Models.DTOs;

namespace Spotify.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PlayHistoryWithSongDTO> _songs = new ObservableCollection<PlayHistoryWithSongDTO>();
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
        public ObservableCollection<GroupedPlayHistory> GroupedSongs
        {
            get => _groupedSongs;
            set
            {
                _groupedSongs = value;
                OnPropertyChanged();
            }
        }

        public HistoryViewModel()
        {
            // What to do ?
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GroupedPlayHistory
    {
        public string Key { get; }
        public ObservableCollection<PlayHistoryWithSongDTO> Items { get; }

        public GroupedPlayHistory(string key, ObservableCollection<PlayHistoryWithSongDTO> items)
        {
            Key = key;
            Items = items;
        }
    }
}