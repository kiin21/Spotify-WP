using Microsoft.UI.Xaml;
using System;

namespace Spotify.Services;

/// <summary>
/// Manages window switching and activation.
/// </summary>
public class WindowManager
{
    private static WindowManager _instance;

    /// <summary>
    /// Gets the singleton instance of the <see cref="WindowManager"/> class.
    /// </summary>
    public static WindowManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WindowManager();
            }
            return _instance;
        }
    }

    private WindowManager() { }

    /// <summary>
    /// Switches to the shell window, closing the login/signup window if it exists.
    /// </summary>
    public void SwitchToShellWindow()
    {
        var app = App.Current as App;

        var newShellWindow = new ShellWindow();
        newShellWindow.Activate();

        app._shellWindow = newShellWindow;

        if (app._loginSignupWindow != null)
        {
            var windowToClose = app._loginSignupWindow;
            app._loginSignupWindow = null;
            windowToClose.Close();
        }
    }

    /// <summary>
    /// Switches to the login/signup window, closing the shell window if it exists.
    /// </summary>
    public void SwitchToLoginSignupWindow()
    {
        var app = App.Current as App;

        var newLoginSignupWindow = new LoginSignupWindow();
        newLoginSignupWindow.Activate();

        app._loginSignupWindow = newLoginSignupWindow;

        if (app._shellWindow != null)
        {
            var windowToClose = app._shellWindow;
            app._shellWindow = null;
            windowToClose.Close();
        }
    }
}