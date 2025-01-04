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

/// <summary>
/// A window that serves as the main shell of the application.
/// </summary>
public sealed partial class ShellWindow : WindowEx
{
    /// <summary>
    /// Gets the main frame of the window.
    /// </summary>
    /// <returns>The main frame.</returns>
    public Frame getMainFrame() => MainFrame;

    /// <summary>
    /// Gets the right sidebar frame of the window.
    /// </summary>
    /// <returns>The right sidebar frame.</returns>
    public Frame getRightSidebarFrame() => RightSidebarFrame;

    private readonly INavigationService _navigationService;
    private readonly QueueService _queueService;

    private ObservableCollection<SongDTO> _queue = new ObservableCollection<SongDTO>();
   
    /// <summary>
    /// Gets or sets the playback queue.
    /// </summary>
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


    /// <summary>
    /// Event to notify subscribers when the queue is updated.
    /// </summary>
    public event EventHandler<QueueUpdatedEventArgs> QueueUpdated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellWindow"/> class.
    /// </summary>

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

    /// <summary>
    /// Initializes the pages of the shell window.
    /// </summary>
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

    /// <summary>
    /// Handles the event when the queue is changed.
    /// </summary>
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

    /// <summary>
    /// Navigates to the specified page type.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <param name="frame">The frame to use for navigation.</param>
    /// <param name="parameter">The parameter to pass to the page.</param>
    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }

    // Method to expose navigation service to other parts of the app
    public INavigationService GetNavigationService() => _navigationService;

}

/// <summary>
/// Provides data for the QueueUpdated event.
/// </summary>
public class QueueUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the updated queue.
    /// </summary>
    public ObservableCollection<SongDTO> UpdatedQueue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueUpdatedEventArgs"/> class.
    /// </summary>
    /// <param name="updatedQueue">The updated queue.</param>
    public QueueUpdatedEventArgs(ObservableCollection<SongDTO> updatedQueue)
    {
        UpdatedQueue = updatedQueue;
    }
}
