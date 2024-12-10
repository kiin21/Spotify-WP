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

    private ObservableCollection<SongDTO> _queue = new ObservableCollection<SongDTO>();
    public ObservableCollection<SongDTO> Queue
    {
        get => _queue;
        set
        {
            if (_queue != value)
            {
                _queue = value;
            }
        }
    }

    // Event to notify subscribers when the queue is updated
    public event EventHandler<QueueUpdatedEventArgs> QueueUpdated;

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

        // Subscribe to the QueueChanged event
        _queueService.QueueChanged += OnQueueChanged;

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

    private async void OnQueueChanged()
    {
        try
        {
            // Reload the queue for the current user
            var updatedQueue = await _queueService.GetQueue();

            // Update the UI thread
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                Queue.Clear();
                foreach (var song in updatedQueue)
                {
                    Queue.Add(song);
                }

                // Create the custom EventArgs with the updated queue
                var eventArgs = new QueueUpdatedEventArgs(Queue);
                QueueUpdated?.Invoke(this, eventArgs); // Notify listeners with the updated queue
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reloading queue: {ex.Message}");
        }
    }



    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }

    // Method to expose navigation service to other parts of the app
    public INavigationService GetNavigationService() => _navigationService;

}

public class QueueUpdatedEventArgs : EventArgs
{
    public ObservableCollection<SongDTO> UpdatedQueue { get; }

    public QueueUpdatedEventArgs(ObservableCollection<SongDTO> updatedQueue)
    {
        UpdatedQueue = updatedQueue;
    }
}
