//LoginPage.xaml.cs:
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;
using Spotify.Services;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Spotify.Views;

/// <summary>
/// A page that handles user login functionality.
/// </summary>
public sealed partial class LoginPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the login page.
    /// </summary>
    public LoginViewModel ViewModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    public LoginPage()
    {
        this.InitializeComponent();
        var userService = (App.Current as App).Services.GetService<UserService>();
        ViewModel = new LoginViewModel(userService);
    }

    /// <summary>
    /// Handles the click event of the login button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
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
                    // Use WindowManager to switch to ShellWindow
                    Spotify.Services.WindowManager.Instance.SwitchToShellWindow();
                }
            }
        }
        catch (Exception ex)
        {
            await ShowContentDialogAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows a content dialog with the specified title and content.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="content">The content of the dialog.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
