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

public sealed partial class LyricPage : Page
{
    public LyricViewModel ViewModel { get; private set; }
    public LyricPage()
    {
        InitializeComponent();
    }

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
    private void OnHighlightedLyricChanged(object sender, LyricLine highlightedLyric)
    {
        if (highlightedLyric != null)
        {
            LyricListView.ScrollIntoView(highlightedLyric, ScrollIntoViewAlignment.Leading);
        }
    }
}