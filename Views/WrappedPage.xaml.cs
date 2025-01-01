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
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class WrappedPage : Page
{
    private WrappedViewModel ViewModel;

    public WrappedPage()
    {
        this.InitializeComponent();
        ViewModel = new WrappedViewModel();
        this.DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<PlayHistoryWithSongDTO> playHistory)
        {
            ViewModel.UpdateData(playHistory);
        }
    }
}