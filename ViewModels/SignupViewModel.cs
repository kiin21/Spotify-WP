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
using Spotify.Contracts.DAO;

namespace Spotify.ViewModels;

/// <summary>
/// ViewModel for managing user sign-up functionality.
/// </summary>
public class SignupViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    private readonly LocalStorageService _localStorageService;
    public event PropertyChangedEventHandler PropertyChanged;
    public RelayCommand GoToSignIn { get; }
    public RelayCommand SignUpCommand { get; }

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

    private string _confirmPassword;
    /// <summary>
    /// Gets or sets the confirmation password.
    /// </summary>
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            OnPropertyChanged(nameof(ConfirmPassword));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignupViewModel"/> class.
    /// </summary>
    /// <param name="userService">The service for managing user data.</param>
    public SignupViewModel(UserService userService)
    {
        _userService = userService;
        _localStorageService = (App.Current as App).Services.GetRequiredService<LocalStorageService>();
        SignUpCommand = new RelayCommand(async param => await ExecuteSignUpAsync());
        GoToSignIn = new RelayCommand(_ => NavigateToLogin());
    }

    /// <summary>
    /// Executes the sign-up process asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple indicating whether the sign-up was successful and a message.</returns>
    /// 

    /// HOT_FIX
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
            var existingUsers = await _userService.GetUsersAsync();
            if (existingUsers.Any(u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.WriteLine("Username already exists");
                return (false, "Username already exists");
            }

            // Hash the password
            var (hashedPassword, salt) = PasswordHasher.HashPassword(Password);

            await _userService.AddUserAsync(Username, Password);
            //FIXXX
            UserDTO user = await _userService.getUserByUsernameAsync(Username);
            QueueService queueService = QueueService.GetInstance(
                                                    App.Current.Services.GetRequiredService<IQueueDAO>(),
                                                    App.Current.Services.GetRequiredService<ISongDAO>(),
                                                    App.Current.CurrentUser);
            await queueService.AddQueueForNewUser(user.Id);

            NavigateToLogin();
            return (true, "Sign up successfully!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Signup error: {ex.Message}");
            return (false, $"An error occurred: {ex.Message}");
        }
    }
    /// End HOT_FIX
    /// <summary>
    /// Validates the user input.
    /// </summary>
    /// <returns>A tuple indicating whether the input is valid and a message.</returns>
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

    /// <summary>
    /// Navigates to the login page.
    /// </summary>
    private void NavigateToLogin()
    {
        var loginSignupWindow = (App.Current as App).LoginSignupWindow as LoginSignupWindow;
        loginSignupWindow?.GetNavigationService().Navigate(typeof(LoginPage));
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
