//INavigationService.cs
using System;
using Microsoft.UI.Xaml.Controls;

namespace Spotify.Contracts.Services;

public interface INavigationService
{
    void Navigate(Type pageType, object parameter = null);
    void Navigate(Type pageType, Frame customFrame, object parameter = null);
}
