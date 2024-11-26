using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class SongDetailPage : Page
{
    private SongDetailViewModel ViewModel { get; set; }
    private PlaybackControlViewModel PlaybackViewModel = PlaybackControlViewModel.Instance;

    public SongDetailPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Get the song passed during navigation
        if (e.Parameter is SongDTO song)
        {
            // Create ViewModel with the song
            ViewModel = new SongDetailViewModel(song);
            DataContext = ViewModel;
        }
    }


    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

}