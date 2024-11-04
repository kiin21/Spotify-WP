//SignupPage.xaml.cs
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class SignupPage : Page
{
    public SignupViewModel ViewModel { get; set; }

    public SignupPage()
    {
        this.InitializeComponent();
    }

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
        ViewModel = new SignupViewModel();
    }

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