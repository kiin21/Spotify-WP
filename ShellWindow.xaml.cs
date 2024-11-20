// ShellWindow.xaml.cs
using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System.Collections.Generic;

namespace Spotify;

public sealed partial class ShellWindow : WindowEx
{

    public Frame getMainFrame()
    {
        return MainFrame;
    }

    public Frame getRightSidebarFrame()
    {
        return RightSidebarFrame;
    }

    private readonly INavigationService _navigationService;

    public ShellWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));
        _navigationService = new NavigationService(MainFrame);
        InitializePages();
    }

    private async void InitializePages()
    {
        // Get the queue after the user logs in
        QueueService _queueService = new QueueService(
               App.Current.Services.GetRequiredService<IQueueDAO>(),
               App.Current.Services.GetRequiredService<ISongDAO>());

        // TODO: Replace with the actual user ID
        string userID = "1234567";
        App.Current.Queue = await _queueService.GetQueueById(userID);


        // Navigation directly
        _navigationService.Navigate(typeof(MainPanelPage), MainFrame);
        _navigationService.Navigate(typeof(HeaderPage), HeaderFrame);
        _navigationService.Navigate(typeof(LeftSidebarPage), LeftSidebarFrame);
        _navigationService.Navigate(typeof(QueuePage), RightSidebarFrame);
        _navigationService.Navigate(typeof(PlaybackControlPage), PlaybackControlsFrame);
    }
    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }
    // Method to expose navigation service to other parts of the app
    public INavigationService GetNavigationService() => _navigationService;

}