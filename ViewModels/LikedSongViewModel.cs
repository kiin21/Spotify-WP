using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Catel.Services;
using Spotify.Models.DTOs;
using Spotify.Services;

namespace Spotify.ViewModels
{
    public class LikedSongViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<LikedSongDTO> _likedSongs;
        private readonly LikedSongService _likedSongService;
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

        public string SongCount => $"{LikedSongs?.Count ?? 0} songs";

        public ObservableCollection<LikedSongDTO> LikedSongs
        {
            get => _likedSongs;
            set
            {
                _likedSongs = value;
                OnPropertyChanged(nameof(LikedSongs));
                OnPropertyChanged(nameof(SongCount));   
            }
        }

        public LikedSongViewModel(LikedSongService likedSongService)
        {
            _likedSongService = likedSongService;
            LikedSongs = new ObservableCollection<LikedSongDTO>();
            LoadLikedSongsAsync();
        }

        private async void LoadLikedSongsAsync()
        {
            // Giả sử bạn có một danh sách songs từ service
            var songs = await _likedSongService.GetLikedSongsAsync();

            // Set index cho từng bài hát
            for (int i = 0; i < songs.Count; i++)
            {
                songs[i].Index = i + 1;
            }
            LikedSongs = new ObservableCollection<LikedSongDTO>(songs);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}