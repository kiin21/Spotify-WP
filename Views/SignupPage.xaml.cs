//SignupPage.xaml.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A page that handles user sign-up functionality.
/// </summary>
public sealed partial class SignupPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the sign-up page.
    /// </summary>
    public SignupViewModel ViewModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignupPage"/> class.
    /// </summary>
    public SignupPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Ensure Frame is not null
        if (Frame == null)
        {
            throw new InvalidOperationException("Frame is null. Ensure this page is hosted in a Frame.");
        }

        // Initialize NavigationService with the Frame
        var navigationService = new NavigationService(Frame);
        var userService = (App.Current as App).Services.GetService<UserService>();
        ViewModel = new SignupViewModel(userService);
    }

    /// <summary>
    /// Handles the click event of the sign-up button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnSignupButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            if (ViewModel.SignUpCommand.CanExecute(null))
            {
                var (success, message) = await ViewModel.ExecuteSignUpAsync();
                if (!success)
                {
                    await ShowContentDialogAsync("Signup Failed", message);
                }
                else
                {
                    await ShowContentDialogAsync("Signup Successful", "You have successfully signed up.");
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
