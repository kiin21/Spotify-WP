//LoginPage.xaml.cs:
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;
using Spotify.Services;
using System.Threading.Tasks;
using System;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; set; }

    public LoginPage()
    {
        this.InitializeComponent();
        ViewModel = new LoginViewModel();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (Frame == null)
        {
            throw new InvalidOperationException("Frame is null. Ensure this page is hosted in a Frame.");
        }
        var navigationService = new NavigationService(Frame);
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