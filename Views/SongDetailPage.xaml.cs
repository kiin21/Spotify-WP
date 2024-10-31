// SongDetailPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class SongDetailPage : Page
{
    public SongDetailViewModel ViewModel { get; }

    public SongDetailPage()
    {
        ViewModel = new SongDetailViewModel();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is SongDTO song)
        {
            ViewModel.Initialize(song);
        }
    }
}