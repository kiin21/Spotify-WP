//SearchResultPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using System.Collections.ObjectModel;

namespace Spotify.Views;

public sealed partial class SearchResultsPage : Page
{
    public SearchResultPageViewModel ViewModel { get; private set; }

    public SearchResultsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Get the navigation service from the shell window
        var shellWindow = (App.Current as App)?.ShellWindow;
        var navigationService = shellWindow?.GetNavigationService();

        if (e.Parameter is ObservableCollection<SongDTO> searchResults)
        {
            ViewModel = new SearchResultPageViewModel(searchResults, navigationService);
            DataContext = ViewModel;
        }
    }
    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            ViewModel.SongSelectedCommand.Execute(song);
        }
    }
}
