using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        private readonly PlaylistService _playlistService;
        private ObservableCollection<PlaylistDTO> _playlists;
        private string _username = "Brian Dang";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string SongCount => $"{Playlists?.Count ?? 0} songs";
        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
                OnPropertyChanged(nameof(SongCount));
            }
        }

        public PlaylistViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
            _playlists = new ObservableCollection<PlaylistDTO>();
            LoadPlaylists();
        }
        private async void LoadPlaylists()
        {
            // Giả sử bạn có một danh sách songs từ service
            var songs = await _playlistService.GetPlaylistsAsync();

            // Set index cho từng bài hát
            for (int i = 0; i < songs.Count; i++)
            {
                songs[i].Index = i + 1;
            }
            Playlists = new ObservableCollection<PlaylistDTO>(songs);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
