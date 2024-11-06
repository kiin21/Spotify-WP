using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spotify.ViewModels
{
    public class LeftSidebarPageViewModel : INotifyPropertyChanged
    {
        private readonly PlaylistService _playlistService;
        private ObservableCollection<PlaylistDTO> _playlists { get; set; }

        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        private PlaylistDTO _selectedPlaylist;
        public PlaylistDTO SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        public LeftSidebarPageViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
            LoadPlaylists();
        }

        private async void LoadPlaylists()
        {
            var playlists = await _playlistService.GetPlaylistsAsync();
            var filteredPlaylists = playlists.Where(p => !p.IsDeleted).ToList();

            Playlists = new ObservableCollection<PlaylistDTO>(filteredPlaylists);
            OnPropertyChanged(nameof(Playlists)); // Thông báo cho UI biết có sự thay đổi
            SelectedPlaylist = Playlists.FirstOrDefault();
        }

        public async Task AddPlaylistAsync(string playlistName)
        {
            var newPlaylist = new PlaylistDTO
            {
                Title = playlistName,
                CreatedBy = "Current User",
                CreatedAt = DateTime.Now,
                IsLikedSong = false,
                IsDeleted = false,
                Avatar = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRXX6SuxBncRoZdYpU9mt8u7hveYoHzeq9vPg&s"
            };

            await _playlistService.AddPlaylistAsync(newPlaylist);
            Playlists.Add(newPlaylist);
        }

        public void RemovePlaylist(string playlistId)
        {
            var playlistToRemove = Playlists.FirstOrDefault(p => p.Id == playlistId);
            if (playlistToRemove != null)
            {
                Playlists.Remove(playlistToRemove);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
