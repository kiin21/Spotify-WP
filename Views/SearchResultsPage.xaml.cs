using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using System.Collections.ObjectModel;

namespace Spotify.Views
{
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

            // Cast the parameter to ObservableCollection<SongDTO>
            if (e.Parameter is ObservableCollection<SongDTO> searchResults)
            {
                ViewModel = new SearchResultPageViewModel(searchResults);
                DataContext = ViewModel;
            }
        }
    }
}
