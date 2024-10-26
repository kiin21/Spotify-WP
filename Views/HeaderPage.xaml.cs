// HeaderPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;
using Spotify.Contracts.Services;

namespace Spotify.Views;
public sealed partial class HeaderPage : Page
{
    public HeaderViewModel ViewModel { get; }

    public HeaderPage()
    {
        this.InitializeComponent();
        // Resolve the SongService from the service provider (DI container)
        var songService = (App.Current as App).Services.GetRequiredService<SongService>();

        // Pass it to the ViewModel
        ViewModel = new HeaderViewModel(songService);
        DataContext = ViewModel;
    }


    private async void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            // Update the suggestions based on user input
            var query = sender.Text;

            // Await the async method
            var suggestions = await ViewModel.GetSuggestions(query);

            sender.ItemsSource = suggestions; // Display suggestions
            if(ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }    
        }
    }

    private void SearchAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.QueryText))
        {
            if (ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }
    }

    private void SearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        // Set the selected suggestion as the search query
        ViewModel.SearchQuery = args.SelectedItem.ToString();
    }

}
