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
            var currentUser = (App.Current as App).CurrentUser;

            if (currentUser == null || string.IsNullOrEmpty(currentUser.Id))
            {
                throw new InvalidOperationException("User is not logged in or user ID is invalid.");
            }

            // Đảm bảo user có playlist "Liked Songs"
            await _playlistService.EnsureLikedSongsPlaylistAsync(currentUser.Id, currentUser.Username);

            // Lấy danh sách playlist của user
            var playlists = await _playlistService.GetPlaylistsByUserIdAsync(currentUser.Id);

            // Lấy danh sách playlist được chia sẻ với user
            var sharedPlaylists = await _playlistService.GetSharedPlaylistsAsync(currentUser.Id);

            // Hợp nhất cả hai danh sách
            var allPlaylists = playlists.Concat(sharedPlaylists).ToList();

            // Gán vào ObservableCollection
            Playlists = new ObservableCollection<PlaylistDTO>(allPlaylists);

            // Chọn playlist đầu tiên (nếu có)
            SelectedPlaylist = Playlists.FirstOrDefault();
        }

        public async Task AddPlaylistAsync(string playlistName)
        {
            var newPlaylist = new PlaylistDTO
            {
                Title = playlistName,
                CreatedBy = (App.Current as App).CurrentUser.Username,
                CreatedAt = DateTime.Now,
                IsLikedSong = false,
                IsDeleted = false,
                Avatar = "ms-appx:///Assets/defaultSong.png",
                OwnerId = (App.Current as App).CurrentUser.Id
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
