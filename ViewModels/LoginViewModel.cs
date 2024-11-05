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
    private readonly UserService _userService;
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

    private bool _rememberMe;
    public bool RememberMe
    {
        get => _rememberMe;
        set
        {
            _rememberMe = value;
            OnPropertyChanged(nameof(RememberMe));
        }
    }

    public RelayCommand GoToSignUp { get; }
    public RelayCommand SignInCommand { get; }

    public LoginViewModel(UserService userService)
    {
        _userService = userService;
        GoToSignUp = new RelayCommand(_ => NavigateToSignUp());
        SignInCommand = new RelayCommand(async param => await ExecuteSignInAsync());

        // Load saved credentials when ViewModel is created
        LoadSavedCredentials();
    }

    public async Task<bool> ExecuteSignInAsync()
    {
        try
        {
            if (!ValidateInput())
                return false;

            if (RememberMe)
            {
                // Save credentials securely
                SaveCredentialsAsync();
            }
            else
            {
                RemoveCredential();
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

                    //// Save current user to local settings
                    //var localSettings = ApplicationData.Current.LocalSettings;
                    //localSettings.Values["currentUsername"] = user.Username;
                    //localSettings.Values["currentUserID"] = user.Id;

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

    private void RemoveCredential()
    {
        var vault = new Windows.Security.Credentials.PasswordVault();
        // Xóa credential cũ nếu có
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
    private void SaveCredentialsAsync()
    {
        var vault = new Windows.Security.Credentials.PasswordVault();
        try
        {
            RemoveCredential();
            // Thêm credential mới
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

    private void LoadSavedCredentials()
    {
        try
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var credentials = vault.FindAllByResource("SpotifyApp");

            if (credentials != null && credentials.Count > 0)
            {
                // Lấy credential đầu tiên
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
            // Không tìm thấy credentials đã lưu
            Debug.WriteLine($"No saved credentials found: {ex.Message}");
            RememberMe = false;
            Username = string.Empty;
            Password = string.Empty;
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}