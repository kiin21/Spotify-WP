using Spotify.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPanelPage : Page
    {
        public MainPanelViewModel ViewModel { get; set; }
        public MainPanelPage()
        {
            this.InitializeComponent();
            ViewModel = (App.Current as App).Services.GetRequiredService<MainPanelViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
