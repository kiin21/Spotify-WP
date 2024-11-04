using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;
using System.Linq;
using System.Diagnostics;
using Spotify.Models.DTOs;

namespace Spotify.Views;

public sealed partial class PlaylistPage : Page
{
    public PlaylistPageViewModel PlaylistPageVM { get; set; }

    public LeftSidebarPageViewModel LeftSidebarPageVM { get; set; }

    public PlaylistPage()
    {
        this.InitializeComponent();

        var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
        var playlistSongService = (App.Current as App).Services.GetRequiredService<PlaylistSongService>();
        LeftSidebarPageVM = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>(); // Khởi tạo LeftSidebarPageVM

        PlaylistPageVM = new PlaylistPageViewModel(playlistService, playlistSongService);
        DataContext = PlaylistPageVM;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string playlistId)
        {
            await PlaylistPageVM.LoadSelectedPlaylist(playlistId);
        }
    }

    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        // TODO: Thêm logic nút Play
    }

    private async void OnRemovePlaylistClick(object sender, RoutedEventArgs e)
    {
        if (PlaylistPageVM.SelectedPlaylist != null)
        {
            // Lưu ID của playlist đang chọn
            var playlistIdToRemove = PlaylistPageVM.SelectedPlaylist.Id;

            // Tìm vị trí của playlist này dựa trên ID
            int currentIndex = LeftSidebarPageVM.Playlists
                .ToList()
                .FindIndex(p => p.Id == playlistIdToRemove);

            // Xóa playlist trong database và cập nhật trạng thái của PlaylistPage
            await PlaylistPageVM.RemoveSelectedPlaylist();

            // Cập nhật giao diện LeftSidebar
            LeftSidebarPageVM.RemovePlaylist(playlistIdToRemove);

            // Tìm playlist gần nhất còn lại
            PlaylistDTO closestPlaylist = null;
            if (currentIndex >= 0 && LeftSidebarPageVM.Playlists.Count > 0)
            {
                if (currentIndex < LeftSidebarPageVM.Playlists.Count)
                {
                    closestPlaylist = LeftSidebarPageVM.Playlists[currentIndex];
                }
                else if (currentIndex - 1 >= 0)
                {
                    closestPlaylist = LeftSidebarPageVM.Playlists[currentIndex - 1];
                }
            }

            // Điều hướng đến playlist gần nhất còn lại
            if (closestPlaylist != null)
            {
                LeftSidebarPageVM.SelectedPlaylist = closestPlaylist;
                await PlaylistPageVM.LoadSelectedPlaylist(closestPlaylist.Id);
            }
        }
    }

}
