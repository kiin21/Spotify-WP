// Login_Signup_Window.xaml.cs
using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace Spotify;

/// <summary>
/// A window that handles user login and signup functionality.
/// </summary>
public sealed partial class LoginSignupWindow : WindowEx
{
    /// <summary>
    /// Gets the main frame of the window.
    /// </summary>
    /// <returns>The main frame.</returns>
    public Frame getMainFrame()
    {
        return MainFrame;
    }

    private readonly INavigationService _navigationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginSignupWindow"/> class.
    /// </summary>
    public LoginSignupWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));

        _navigationService = new NavigationService(MainFrame);
        MainFrame.Navigate(typeof(LoginPage));
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

    /// <summary>
    /// Gets the navigation service.
    /// </summary>
    /// <returns>The navigation service.</returns>
    public INavigationService GetNavigationService() => _navigationService;
}
