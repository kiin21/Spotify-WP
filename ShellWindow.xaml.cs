using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Spotify.Contracts.DAO;
using System.Collections.ObjectModel;
using Spotify.Models.DTOs;

namespace Spotify;

public sealed partial class ShellWindow : WindowEx
{
    public Frame getMainFrame() => MainFrame;

    public Frame getRightSidebarFrame() => RightSidebarFrame;

    private readonly INavigationService _navigationService;
    private readonly QueueService _queueService;
    public ObservableCollection<SongDTO> Queue{get; set; }

    public ShellWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));

        // Initialize navigation service
        _navigationService = new NavigationService(MainFrame);

        // Retrieve singleton instance of QueueService from DI container
        _queueService = QueueService.GetInstance(
            App.Current.Services.GetRequiredService<IQueueDAO>(),
            App.Current.Services.GetRequiredService<ISongDAO>(),
            App.Current.CurrentUser);

        InitializePages();
    }

    private async void InitializePages()
    {
        try
        {
            // Initialize PlaybackControlService with the dispatcher from the UI thread
            PlaybackControlService.Initialize(DispatcherQueue.GetForCurrentThread());

            // Load the queue for the current user
            Queue = await _queueService.GetQueue();

            // Navigate to initial pages
            _navigationService.Navigate(typeof(MainPanelPage), MainFrame);
            _navigationService.Navigate(typeof(HeaderPage), HeaderFrame);
            _navigationService.Navigate(typeof(LeftSidebarPage), LeftSidebarFrame);
            _navigationService.Navigate(typeof(RightSideBarPage), RightSidebarFrame);
            _navigationService.Navigate(typeof(PlaybackControlPage), PlaybackControlsFrame);
        }
        catch (Exception ex)
        {
            // Handle exceptions, log errors, or notify the user
            Console.WriteLine($"Error initializing pages: {ex.Message}");
        }
    }

    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }

    // Method to expose navigation service to other parts of the app
    public INavigationService GetNavigationService() => _navigationService;
}
