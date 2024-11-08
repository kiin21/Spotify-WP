using Spotify.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;
using Spotify.Models.DTOs;

namespace Spotify.Views;
public sealed partial class MainPanelPage : Page
{
    public MainPanelViewModel ViewModel { get; set; }
    public MainPanelPage()
    {
        this.InitializeComponent();
        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        ViewModel = new MainPanelViewModel(songService);
        this.DataContext = ViewModel;
    }

    private void Item_Selected(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            Frame.Navigate(typeof(SongDetailPage), song);
        }
    }
}
