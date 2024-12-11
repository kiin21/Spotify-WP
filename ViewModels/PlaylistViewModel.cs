
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
    /// <summary>  
    /// ViewModel for managing playlists.  
    /// </summary>  
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        private readonly PlaylistService _playlistService;
        private ObservableCollection<PlaylistDTO> _playlists;
        private PlaylistDTO _selectedPlaylist;

        /// <summary>  
        /// Gets or sets the collection of playlists.  
        /// </summary>  
        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        /// <summary>  
        /// Gets or sets the selected playlist.  
        /// </summary>  
        public PlaylistDTO SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
                // Trigger the SelectedPlaylistIdChanged event when SelectedPlaylist changes  
                SelectedPlaylistIdChanged?.Invoke(this, _selectedPlaylist?.Id.ToString());
            }
        }

        /// <summary>  
        /// Occurs when the selected playlist ID changes.  
        /// </summary>  
        public event EventHandler<string> SelectedPlaylistIdChanged;

        /// <summary>  
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.  
        /// </summary>  
        /// <param name="playlistService">The service for managing playlists.</param>  
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

        /// <summary>  
        /// Clears the selected playlist and other data.  
        /// </summary>  
        public void ClearData()
        {
            SelectedPlaylist = null;
            // Clear any other data that needs to be reset  
        }

        /// <summary>  
        /// Loads the selected playlist by its ID asynchronously.  
        /// </summary>  
        /// <param name="playlistId">The ID of the playlist to load.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
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
}
