// ShellWindow.xaml.cs
using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;

namespace Spotify;

public sealed partial class ShellWindow : WindowEx
{
    
    public Frame getMainFrame()
    {
        return MainFrame;
    }
    private readonly INavigationService _navigationService;
    public ShellWindow()
    {
        InitializeComponent();
        
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));

        _navigationService = new NavigationService(MainFrame);

        InitializePages();
    }

    private void InitializePages()
    {
        // No need for Initialize method, just use navigation directly
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