/// <summary>
/// Provides navigation capabilities for a WinUI application using frames.
/// </summary>
using System;
using Microsoft.UI.Xaml.Controls;
using Spotify.Contracts.Services;

namespace Spotify.Services;

public class NavigationService : INavigationService
{
    private readonly Frame _rootFrame;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class with the specified root frame.
    /// </summary>
    /// <param name="frame">The root frame used for navigation.</param>
    public NavigationService(Frame frame)
    {
        _rootFrame = frame;
    }

    /// <summary>
    /// Navigates to the specified page type using the root frame.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <param name="parameter">Optional parameter to pass to the target page.</param>
    /// <exception cref="InvalidOperationException">Thrown if the root frame is not initialized.</exception>
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

    /// <summary>
    /// Navigates to the specified page type using a custom frame.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <param name="customFrame">The custom frame to use for navigation.</param>
    /// <param name="parameter">Optional parameter to pass to the target page.</param>
    /// <exception cref="ArgumentNullException">Thrown if the custom frame is null.</exception>
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
