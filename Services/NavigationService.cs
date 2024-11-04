// NavigationService.cs
using System;
using Microsoft.UI.Xaml.Controls;
using Spotify.Contracts.Services;

namespace Spotify.Services;

public class NavigationService : INavigationService
{
    private readonly Frame _rootFrame;

    public NavigationService(Frame frame)
    {
        _rootFrame = frame;
    }

    // Navigate using the root frame
    public void Navigate(Type pageType, object parameter = null)
    {
        if (_rootFrame != null)
        {
            _rootFrame.Navigate(pageType, parameter);
        }
        else
        {
            throw new InvalidOperationException("Navigation frame is not initialized.");
        }
    }

    // Overloaded Navigate method for a custom frame
    public void Navigate(Type pageType, Frame customFrame, object parameter = null)
    {
        if (customFrame != null)
        {
            customFrame.Navigate(pageType, parameter);
        }
        else
        {
            throw new ArgumentNullException(nameof(customFrame), "Custom frame cannot be null.");
        }
    }
}
