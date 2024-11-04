using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Spotify.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
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
                OnPropertyChanged(nameof(Playlists));
            }
        }

        public PlaylistDTO SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
                // Kích hoạt sự kiện SelectedPlaylistIdChanged khi SelectedPlaylist thay đổi
                SelectedPlaylistIdChanged?.Invoke(this, _selectedPlaylist?.Id.ToString());
            }
        }

        // Khai báo sự kiện SelectedPlaylistIdChanged
        public event EventHandler<string> SelectedPlaylistIdChanged;

        public PlaylistViewModel(PlaylistService playlistService)
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
        }

        public void ClearData()
        {
            SelectedPlaylist = null;
            // Clear any other data that needs to be reset
        }

        public async Task LoadSelectedPlaylist(string playlistId)
        {
            if (string.IsNullOrEmpty(playlistId))
                return;

            try
            {
                var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
                if (playlist != null)
                {
                    SelectedPlaylist = playlist;
                    // Load additional data if needed
                }
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                Debug.WriteLine($"Error loading playlist: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
