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
        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        ViewModel = new HeaderViewModel(songService);
        DataContext = ViewModel;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel.SearchCommand.CanExecute(null))
        {
            ViewModel.SearchCommand.Execute(null);
        }
    }

    private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            if (ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }
    }
}
