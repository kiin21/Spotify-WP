using Microsoft.Extensions.DependencyInjection;
using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.ViewModels
{
    public class SharePlaylistViewModel
    {
        public ObservableCollection<UserShareDTO> Users { get; set; }
        private readonly PlaylistDTO _playlist;
        private readonly UserService _userService;
        private readonly PlaylistService _playlistService;

        public SharePlaylistViewModel(PlaylistDTO playlist)
        {
            _playlist = playlist;

            // Lấy UserService và PlaylistService từ App container
            _userService = (App.Current as App).Services.GetRequiredService<UserService>();
            _playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();

            Users = new ObservableCollection<UserShareDTO>();
            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();

            // Loai bỏ user hiện tại khỏi danh sách
            var currentUserId = (App.Current as App).CurrentUser.Id;
            users = users.Where(u => u.Id != currentUserId).ToList();

            foreach (var user in users)
            {
                Users.Add(new UserShareDTO
                {
                    UserId = user.Id,
                    Username = user.Username,
                    IsSelected = false
                });
            }
        }

        public async Task SharePlaylistAsync()
        {
            var selectedUsers = Users.Where(u => u.IsSelected).ToList();
            foreach (var user in selectedUsers)
            {
                await _playlistService.SharePlaylistAsync(_playlist.Id, user.UserId);
            }
        }
    }

    public class UserShareDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool IsSelected { get; set; }
    }
}
