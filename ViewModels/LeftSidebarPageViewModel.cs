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
        private ObservableCollection<PlaylistDTO> _playlists;
        private PlaylistDTO _selectedPlaylist;

        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged();
            }
        }

        public PlaylistDTO SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged();
            }
        }

        public LeftSidebarPageViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
            _playlists = new ObservableCollection<PlaylistDTO>();
            LoadPlaylists();
        }

        private async void LoadPlaylists()
        {
            var playlists = await _playlistService.GetPlaylistsAsync();
            var filteredPlaylists = playlists.Where(p => !p.IsDeleted).ToList();
            Playlists = new ObservableCollection<PlaylistDTO>(filteredPlaylists);
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

        private readonly PlaylistPageViewModel _playlistPageViewModel;

        public LeftSidebarPageViewModel(PlaylistService playlistService, PlaylistPageViewModel playlistPageViewModel)
        {
            _playlistService = playlistService;
            _playlistPageViewModel = playlistPageViewModel;
            _playlists = new ObservableCollection<PlaylistDTO>();
            LoadPlaylists();

            // Đăng ký sự kiện PlaylistRemoved
            _playlistPageViewModel.PlaylistRemoved += OnPlaylistRemoved;
        }

        // Phương thức xử lý sự kiện
        private void OnPlaylistRemoved(object sender, string playlistId)
        {
            var playlistToRemove = Playlists.FirstOrDefault(p => p.Id == playlistId);
            if (playlistToRemove != null)
            {
                Playlists.Remove(playlistToRemove);
                // Cập nhật SelectedPlaylist nếu playlist bị xóa là playlist đang chọn
                if (SelectedPlaylist == playlistToRemove)
                {
                    SelectedPlaylist = Playlists.FirstOrDefault();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
