// LyricPage.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Spotify.Views;

/// <summary>
/// A page that displays the lyrics of the currently playing song.
/// </summary>
public sealed partial class LyricPage : Page
{
    /// <summary>
    /// Gets the view model for the lyric page.
    /// </summary>
    public LyricViewModel ViewModel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricPage"/> class.
    /// </summary>
    public LyricPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is SongDTO song)
        {
            ViewModel = new LyricViewModel(song);
            ViewModel.LoadLyrics();
            ViewModel.HighlightedLyricChanged += OnHighlightedLyricChanged;
        }
    }

    /// <summary>
    /// Handles the event when the highlighted lyric changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="highlightedLyric">The highlighted lyric line.</param>
    private void OnHighlightedLyricChanged(object sender, LyricLine highlightedLyric)
    {
        if (highlightedLyric != null)
        {
            LyricListView.ScrollIntoView(highlightedLyric, ScrollIntoViewAlignment.Leading);
        }
    }
}
