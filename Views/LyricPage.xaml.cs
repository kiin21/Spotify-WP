// LyricPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class LyricPage : Page
{
    public LyricViewModel ViewModel { get; private set; }

    public LyricPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is SongDTO song)
        {
            ViewModel = new LyricViewModel(song);
            ViewModel.LoadLyrics();
        }
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}