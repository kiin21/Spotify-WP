// ShellWindow.xaml.cs
using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Spotify;

public sealed partial class ShellWindow : WindowEx
{
    
    public Frame getMainFrame()
    {
        return MainFrame;
    }

    public ShellWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));

        InitializePages();
    }

    private void InitializePages()
    {
        // No need for Initialize method, just use navigation directly
        NavigateToPage(typeof(MainPanelPage), MainFrame);
        NavigateToPage(typeof(HeaderPage), HeaderFrame);
        NavigateToPage(typeof(LeftSidebarPage), LeftSidebarFrame);
        NavigateToPage(typeof(QueuePage), RightSidebarFrame);
        NavigateToPage(typeof(PlaybackControlPage), PlaybackControlsFrame);
    }
    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }

}