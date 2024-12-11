// Services/WindowManager.cs
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

        // Create and display ShellWindow
        if (app._shellWindow == null)
        {
            app._shellWindow = new ShellWindow();
        }
        app._shellWindow.Activate();

        // After ShellWindow is activated, close the old window
        if (app._loginSignupWindow != null)
        {
            app._loginSignupWindow.Close();
        }
    }
}

