using Catel.MVVM;
using Microsoft.UI.Xaml.Controls;

namespace Spotify.Views;

/// <summary>
/// A page that displays a success message.
/// </summary>
public sealed partial class SuccessPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessPage"/> class.
    /// </summary>
    public SuccessPage()
    {
        this.InitializeComponent();
    }

    private void BackToHome_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var shellWindow = App.Current.ShellWindow;
        shellWindow.GetNavigationService().Navigate(typeof(MainPanelPage));
    }
}
