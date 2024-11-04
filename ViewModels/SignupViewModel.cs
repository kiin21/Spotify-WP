//SignupViewModel.cs
using Spotify.Helpers;
using Spotify.Views;
using Spotify;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Spotify.Models.DTOs;
using Spotify.Services;

namespace Spotify.ViewModels;
public class SignupViewModel : INotifyPropertyChanged
{
    private readonly LocalStorageService _localStorageService;
    public event PropertyChangedEventHandler PropertyChanged;
    public RelayCommand GoToSignIn { get; }
    public RelayCommand SignUpCommand { get; }

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

    private string _confirmPassword;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            OnPropertyChanged(nameof(ConfirmPassword));
        }
    }

    public SignupViewModel()
    {
        _localStorageService = (App.Current as App).Services.GetRequiredService<LocalStorageService>();
        SignUpCommand = new RelayCommand(async param => await ExecuteSignUpAsync());
        GoToSignIn = new RelayCommand(_ => NavigateToLogin());
    }

    public async Task<(bool success, string message)> ExecuteSignUpAsync()
    {
        try
        {
            var (isValid, message) = ValidateInput();
            if (!isValid)
            {
                return (false, message);
            }

            // Check if username already exists
            var existingUsers = await _localStorageService.GetUsersAsync();
            if (existingUsers.Any(u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.WriteLine("Username already exists");
                return (false, "Username already exists");
            }

            // Hash the password
            var (hashedPassword, salt) = PasswordHasher.HashPassword(Password);

            var user = new UserDTO
            {
                Username = Username,
                HashedPassword = hashedPassword,
                Salt = salt
            };

            await _localStorageService.SaveUserAsync(user);
            NavigateToLogin();
            return (true, "Sign up successfully!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Signup error: {ex.Message}");
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    private (bool, string) ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return (false, "Username is required");
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            return (false, "Password is required");
        }
        if (string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            return (false, "Confirm password is required");
        }
        if (Password != ConfirmPassword)
        {
            return (false, "Passwords do not match");
        }
        return (true, "Sign up successfully!");
    }

    private void NavigateToLogin()
    {
        var loginSignupWindow = (App.Current as App).LoginSignupWindow as LoginSignupWindow;
        loginSignupWindow?.GetNavigationService().Navigate(typeof(LoginPage));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}