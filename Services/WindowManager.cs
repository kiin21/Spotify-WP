// Services/WindowManager.cs
using Microsoft.UI.Xaml;
using System;

namespace Spotify.Services;

public class WindowManager
{
    private static WindowManager _instance;

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

    public void SwitchToShellWindow()
    {
        var app = App.Current as App;

        // Tạo và hiển thị ShellWindow
        if (app._shellWindow == null)
        {
            app._shellWindow = new ShellWindow();
        }
        app._shellWindow.Activate();

        // Sau khi ShellWindow được kích hoạt, đóng cửa sổ cũ
        if (app._loginSignupWindow != null)
        {
            app._loginSignupWindow.Close();
        }
    }

}