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


public sealed partial class HistoryPage : Page
{
    public HistoryViewModel ViewModel { get; set; }
    public HistoryPage()
    {
        this.InitializeComponent();
        ViewModel = new HistoryViewModel();
        this.DataContext = ViewModel;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<PlayHistoryWithSongDTO> list)
        {  
            ViewModel.Songs = new ObservableCollection<PlayHistoryWithSongDTO>(list);
        }
    }

    private void Item_Selected(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            Frame.Navigate(typeof(SongDetailPage), song);
        }
    }
}
