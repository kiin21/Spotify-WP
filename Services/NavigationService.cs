using System;
using Microsoft.UI.Xaml.Controls;

namespace Spotify.Services;

public interface INavigationService
{
    void Navigate(Type pageType, object parameter = null);
}

public class NavigationService : INavigationService
{
    private readonly Frame _frame;

    public NavigationService(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate(Type pageType, object parameter = null)
    {
        _frame.Navigate(pageType, parameter);
    }
}