// NavigationService.cs
using System;
using Microsoft.UI.Xaml.Controls;
using Spotify.Contracts.Services;

namespace Spotify.Services;

/// <summary>
/// Service class for handling navigation operations.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Frame _rootFrame;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
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
    /// <param name="parameter">The parameter to pass to the page. Default is null.</param>
    /// <exception cref="InvalidOperationException">Thrown when the root frame is not initialized.</exception>
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
    /// <param name="customFrame">The custom frame used for navigation.</param>
    /// <param name="parameter">The parameter to pass to the page. Default is null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the custom frame is null.</exception>
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
