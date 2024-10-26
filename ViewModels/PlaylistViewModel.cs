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

        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        public PlaylistViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
            _playlists = new ObservableCollection<PlaylistDTO>();
            LoadPlaylists();
        }

        public async void LoadPlaylists()
        {
            var playlists = await _playlistService.GetPlaylistsAsync();
            Playlists = new ObservableCollection<PlaylistDTO>(playlists);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
