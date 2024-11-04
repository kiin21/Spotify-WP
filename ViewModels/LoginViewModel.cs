// LoginViewModel.cs
using Microsoft.UI.Xaml.Controls;
using Spotify.Helpers;
using Spotify.Views;
using Spotify;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Windows.Storage;
using Spotify.Services;

namespace Spotify.ViewModels;
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly LocalStorageService _localStorageService;
    public event PropertyChangedEventHandler PropertyChanged;

    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public RelayCommand GoToSignUp { get; }
    public RelayCommand SignInCommand { get; }

    public LoginViewModel()
    {
        _localStorageService = (App.Current as App).Services.GetRequiredService<LocalStorageService>();
        GoToSignUp = new RelayCommand(_ => NavigateToSignUp());
        SignInCommand = new RelayCommand(async param => await ExecuteSignInAsync());
    }

    public async Task<bool> ExecuteSignInAsync()
    {
        try
        {
            if (!ValidateInput())
                return false;

            var users = await _localStorageService.GetUsersAsync();

            // First find user by username only
            var user = users.FirstOrDefault
                (u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                // Verify the password using the stored salt and hash
                bool isValidPassword = PasswordHasher.VerifyPassword(
                    Password,
                    user.HashedPassword,
                    user.Salt
                );

                if (isValidPassword)
                {
                    // Login successful
                    Debug.WriteLine("Login successfully");

                    //// Save current user to local settings
                    var localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["currentUser"] = user.Username;
                    // TODO: Navigate to main window
                    return true;
                }
                else
                {
                    Debug.WriteLine("Invalid password");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("User not found");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            return false;
        }
        return true;
    }

    private void NavigateToSignUp()
    {
        var loginSignupWindow = (App.Current as App).LoginSignupWindow as LoginSignupWindow;
        loginSignupWindow?.GetNavigationService().Navigate(typeof(SignupPage));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}