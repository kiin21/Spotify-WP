//LoginPage.xaml.cs:
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;
using Spotify.Services;
using System.Threading.Tasks;
using System;


namespace Spotify.Views;
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; set; }

    public LoginPage()
    {
        this.InitializeComponent();
        ViewModel = new LoginViewModel();
    }

    private async void OnLoginButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            if (ViewModel.SignInCommand.CanExecute(null))
            {
                bool result = await ViewModel.ExecuteSignInAsync();
                if (!result)
                {
                    await ShowContentDialogAsync("Login Failed", "Invalid username or password.");
                }
                else
                {
                    await ShowContentDialogAsync("Login Successful", "You have successfully logged in.");
                    // Sử dụng WindowManager để chuyển đổi sang ShellWindow
                    Spotify.Services.WindowManager.Instance.SwitchToShellWindow();
                }
            }
        }
        catch (Exception ex)
        {
            await ShowContentDialogAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    private async Task ShowContentDialogAsync(string title, string content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            CloseButtonText = "Ok",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}