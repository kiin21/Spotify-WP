// INavigationService.cs
using System;
using Microsoft.UI.Xaml.Controls;

namespace Spotify.Contracts.Services;

/// <summary>
/// Interface for navigation service
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified page type.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <param name="parameter">The parameter to pass to the page.</param>
    void Navigate(Type pageType, object parameter = null);

    /// <summary>
    /// Navigates to the specified page type using a custom frame.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <param name="customFrame">The custom frame to use for navigation.</param>
    /// <param name="parameter">The parameter to pass to the page.</param>
    void Navigate(Type pageType, Frame customFrame, object parameter = null);
}
