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

/// <summary>
/// ViewModel for managing user login functionality.
/// </summary>
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    public event PropertyChangedEventHandler PropertyChanged;

    private string _username;
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    private bool _rememberMe;
    /// <summary>
    /// Gets or sets a value indicating whether to remember the user's credentials.
    /// </summary>
    public bool RememberMe
    {
        get => _rememberMe;
        set
        {
            _rememberMe = value;
            OnPropertyChanged(nameof(RememberMe));
        }
    }

    /// <summary>
    /// Gets the command to navigate to the sign-up page.
    /// </summary>
    public RelayCommand GoToSignUp { get; }

    /// <summary>
    /// Gets the command to sign in the user.
    /// </summary>
    public RelayCommand SignInCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="userService">The service for managing user data.</param>
    public LoginViewModel(UserService userService)
    {
        _userService = userService;
        GoToSignUp = new RelayCommand(_ => NavigateToSignUp());
        SignInCommand = new RelayCommand(async param => await ExecuteSignInAsync());

        // Load saved credentials when ViewModel is created
        LoadSavedCredentials();
    }

    /// <summary>
    /// Executes the sign-in process asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the sign-in was successful.</returns>
    public async Task<bool> ExecuteSignInAsync()
    {
        try
        {
            if (!ValidateInput())
                return false;

            // Remove old credentials
            RemoveCredential();
            if (RememberMe)
            {
                SaveCredentials();
            }

            var users = await _userService.GetUsersAsync();

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

                    App.Current.CurrentUser = user;
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

    /// <summary>
    /// Validates the user input.
    /// </summary>
    /// <returns>A boolean indicating whether the input is valid.</returns>
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

    /// <summary>
    /// Navigates to the sign-up page.
    /// </summary>
    private void NavigateToSignUp()
    {
        var loginSignupWindow = (App.Current as App).LoginSignupWindow as LoginSignupWindow;
        loginSignupWindow?.GetNavigationService().Navigate(typeof(SignupPage));
    }

    /// <summary>
    /// Removes the saved credentials.
    /// </summary>
    private void RemoveCredential()
    {
        var vault = new Windows.Security.Credentials.PasswordVault();
        // Remove old credentials if any
        try
        {
            var oldCredentials = vault.FindAllByResource("SpotifyApp");
            foreach (var cred in oldCredentials)
            {
                vault.Remove(cred);
            }
        }
        catch { }
    }

    /// <summary>
    /// Saves the user's credentials.
    /// </summary>
    private void SaveCredentials()
    {
        var vault = new Windows.Security.Credentials.PasswordVault();
        try
        {
            RemoveCredential();
            // Add new credentials
            vault.Add(new Windows.Security.Credentials.PasswordCredential(
                "SpotifyApp",
                Username,
                Password
            ));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving credentials: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the saved credentials.
    /// </summary>
    private void LoadSavedCredentials()
    {
        try
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var credentials = vault.FindAllByResource("SpotifyApp");

            if (credentials != null && credentials.Count > 0)
            {
                // Get the first credential
                var credential = credentials[0];
                credential.RetrievePassword();

                Username = credential.UserName;
                Password = credential.Password;
                RememberMe = true;

                Debug.WriteLine($"Loaded saved credentials for user: {Username}");
            }
        }
        catch (Exception ex)
        {
            // No saved credentials found
            Debug.WriteLine($"No saved credentials found: {ex.Message}");
            RememberMe = false;
            Username = string.Empty;
            Password = string.Empty;
        }
    }

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
