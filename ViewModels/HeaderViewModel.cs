// HeaderViewModel.cs
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Windows.UI.Notifications;

namespace Spotify.ViewModels;

public partial class HeaderViewModel : INotifyPropertyChanged
{
    private string _searchQuery;
    private readonly SongService _songService;
    private ObservableCollection<SongDTO> _searchResults;
    public string UserAvatar { get; set; } = App.Current.CurrentUser.UserAvatar;
    //public string UserAvatar { get; set; } = "../Assets/defaultAvt.jpg";

    private readonly ArtistService _artistService;
    private readonly UserService _userService;

    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged(nameof(SearchQuery));
        }
    }

    public ObservableCollection<NotificationDTO> Notifications { get; } = new ObservableCollection<NotificationDTO>();

    private bool _hasNotification;

    public bool HasNotification
    {
        get => _hasNotification;
        set
        {
            _hasNotification = value;
            OnPropertyChanged(nameof(HasNotification));
        }
    }

    public ICommand SearchCommand { get; }


    //public HeaderViewModel(SongService songService)
    //{
    //    SearchCommand = new RelayCommand(async _ => await ExecuteSearchAsync(), _ => CanExecuteSearch());
    //    _songService = songService;
    //    SearchResults = new ObservableCollection<SongDTO>();
    //}

    public HeaderViewModel(SongService songService, ArtistService artistService, UserService userService)
    {
        SearchCommand = new RelayCommand(async _ => await ExecuteSearchAsync(), _ => CanExecuteSearch());
        _songService = songService;
        SearchResults = new ObservableCollection<SongDTO>();

        _artistService = artistService;
        _userService = userService;
    }


    private async Task ExecuteSearchAsync()
    {
        if (SearchQuery == "")
        {
            var results = await _songService.GetAllSongs();
            SearchResults = new ObservableCollection<SongDTO>(results);
        }
        else
        {

            var results = await _songService.SearchSongs(SearchQuery);
            SearchResults = new ObservableCollection<SongDTO>(results);
        }

        var shellWindow = (App.Current as App).ShellWindow;
        var mainFrame = shellWindow.getMainFrame();
        shellWindow.GetNavigationService().Navigate(typeof(SearchResultsPage), SearchResults);
    }

    private bool CanExecuteSearch()
    {
        return SearchQuery != null;
    }

    public async Task CheckForSongUpdatesAsync()
    {
        var currentUser = App.Current.CurrentUser;
        if (currentUser == null || currentUser.FollowArtist == null) return;

        bool hasNewSongs = false;
        var newNotifications = new ObservableCollection<NotificationDTO>();

        // Nếu không có nghệ sĩ nào được follow
        if (!currentUser.FollowArtist.Any())
        {
            if (!Notifications.Any(n => n.Message == "Follow some artists to get updates!"))
            {
                Notifications.Add(new NotificationDTO
                {
                    Message = "Follow some artists to get updates!",
                    Timestamp = DateTime.Now,
                    ArtistName = "",
                    ArtistId = "",
                });
            }
            return;
        }

        foreach (var artistId in currentUser.FollowArtist)
        {
            var artist = await _artistService.GetArtistByIdAsync(artistId);
            if (artist == null) continue;

            // Lấy cache hiện tại
            var cachedSongs = _songService.GetCachedSongs(artistId);

            // Nếu cache rỗng, không coi tất cả là bài hát mới
            if (!cachedSongs.Any())
            {
                _songService.UpdateCache(artistId, artist.SongIds);
                continue;
            }

            // Kiểm tra bài hát mới
            var newSongIds = artist.SongIds.Except(cachedSongs).ToList();

            if (newSongIds.Any())
            {
                hasNewSongs = true;

                // Lấy chi tiết các bài hát mới
                foreach (var songId in newSongIds)
                {
                    var newSong = await _songService.GetSongByIdAsync(songId);

                    // Tạo thông báo mới
                    var notification = new NotificationDTO
                    {
                        Message = $"'{newSong.title}' was just released by {artist.Name}",
                        ArtistId = artist.Id.ToString(),
                        ArtistName = artist.Name,
                        Timestamp = DateTime.Now
                    };

                    newNotifications.Add(notification);
                }

                // Cập nhật cache
                _songService.UpdateCache(artistId, artist.SongIds);
            }
        }

        // Nếu không có thông báo mới, giữ nguyên các thông báo cũ
        if (!hasNewSongs)
        {
            // Nếu chưa có thông báo "No new updates", thêm vào
            if (!Notifications.Any())
            {
                Notifications.Add(new NotificationDTO
                {
                    Message = "No messages",
                    ArtistName = "",
                    ArtistId = "",
                    Timestamp = DateTime.Now
                });
            }
        }
        else
        {
            // Xóa thông báo mặc định nếu có
            var noUpdateNotification = Notifications.FirstOrDefault(n => n.Message == "No messages");
            if (noUpdateNotification != null)
            {
                Notifications.Remove(noUpdateNotification);
            }

            // Thêm các thông báo mới vào đầu danh sách
            foreach (var notification in newNotifications.Reverse())
            {
                Notifications.Insert(0, notification);
            }

        }

        HasNotification = hasNewSongs;
        OnPropertyChanged(nameof(Notifications));
        OnPropertyChanged(nameof(HasNotification));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
