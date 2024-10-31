// SongDetailViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Spotify.Models.DTOs;

namespace Spotify.ViewModels;

public partial class SongDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string moreInfo = string.Empty;   
    
    [ObservableProperty]
    private string lyrics = string.Empty;

    [ObservableProperty]
    private string artistInfo = string.Empty;

    [ObservableProperty]
    private string imageUrl = string.Empty;

    public void Initialize(SongDTO songInfo)
    {
        Title = songInfo.title;
        Lyrics = songInfo.plainLyrics;
        MoreInfo = $"{songInfo.ArtistName} • {songInfo.ReleaseDate} • {songInfo.Duration}";
        ArtistInfo = $"{songInfo.ArtistName}";
        ImageUrl = songInfo.CoverArtUrl ?? "ms-appx:///Assets/Image1.jpg";
    }
}