using Spotify.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
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
}
