using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A page that displays the play history.
/// </summary>
public sealed partial class HistoryPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the history page.
    /// </summary>
    public HistoryViewModel ViewModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryPage"/> class.
    /// </summary>
    public HistoryPage()
    {
        this.InitializeComponent();
        ViewModel = new HistoryViewModel();
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<PlayHistoryWithSongDTO> list)
        {
            ViewModel.Songs = new ObservableCollection<PlayHistoryWithSongDTO>(list);
        }
    }

    /// <summary>
    /// Handles the item click event for the play history list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Item_Selected(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PlayHistoryWithSongDTO song)
        {
            Frame.Navigate(typeof(SongDetailPage), song.SongDetails);
        }
    }
}