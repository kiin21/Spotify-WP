using Catel.MVVM;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Microsoft.UI.Xaml.Navigation;
using Spotify.ViewModels;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System.Linq;
using System;

namespace Spotify.Views;

/// <summary>
/// A page that displays the user's wrapped statistics.
/// </summary>
public sealed partial class WrappedPage : Page
{
    /// <summary>
    /// Gets the view model for the wrapped page.
    /// </summary>
    private WrappedViewModel ViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedPage"/> class.
    /// </summary>
    public WrappedPage()
    {
        this.InitializeComponent();
        ViewModel = new WrappedViewModel();
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<PlayHistoryWithSongDTO> playHistory)
        {
            ViewModel.UpdateData(playHistory);
        }
    }
}

