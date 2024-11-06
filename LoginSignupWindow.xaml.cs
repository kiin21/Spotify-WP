// Login_Signup_Window.xaml.cs
using System;
using System.IO;
using Spotify.Views;
using Spotify.Contracts.Services;
using Spotify.Services;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace Spotify;

public sealed partial class LoginSignupWindow : WindowEx
{

    public Frame getMainFrame()
    {
        return MainFrame;
    }
    private readonly INavigationService _navigationService;
    public LoginSignupWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));

        _navigationService = new NavigationService(MainFrame);
        MainFrame.Navigate(typeof(LoginPage));
    }

    public void NavigateToPage(Type pageType, Frame frame, object parameter = null)
    {
        frame.Navigate(pageType, parameter);
    }
    // Method to expose navigation service to other parts of the app
    public INavigationService GetNavigationService() => _navigationService;

}