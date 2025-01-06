using CommunityToolkit.Mvvm.Input;
using Spotify.Contracts.Services;
using Spotify.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System;
using Spotify.Models.DTOs;
using Spotify;
using Spotify.Contracts.DAO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Views;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// ViewModel for managing payment processing.
/// </summary>
public class PaymentViewModel : INotifyPropertyChanged
{
    public string PremiumType;

    private bool _isStripeSelected;
    /// <summary>
    /// Gets or sets a value indicating whether Stripe is selected as the payment method.
    /// </summary>
    public bool IsStripeSelected
    {
        get => _isStripeSelected;
        set => SetProperty(ref _isStripeSelected, value);
    }

    private bool _isPaypalSelected;

    public bool IsPaypalSelected
    {
        get => _isPaypalSelected;
        set => SetProperty(ref _isPaypalSelected, value);
    }

    private bool _isPremium;
    /// <summary>
    /// Gets or sets a value indicating whether the user is a premium user.
    /// </summary>
    public bool IsPremium
    {
        get => _isPremium;
        set
        {
            _isPremium = value;
            OnPropertyChanged(nameof(IsPremium));
        }
    }

    /// <summary>
    /// Gets the command to choose Stripe as the payment method.
    /// </summary>
    public ICommand ChooseStripeCommand { get; }

    /// <summary>
    /// Gets the command to choose PayPal as the payment method.
    /// </summary>
    public ICommand ChoosePaypalCommand { get; }

    /// <summary>
    /// Gets the command to process the payment.
    /// </summary>
    public ICommand PayCommand { get; }

    private string _cardNumber;
    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    public string CardNumber
    {
        get => _cardNumber;
        set => SetProperty(ref _cardNumber, value);
    }

    private string _cvv;
    /// <summary>
    /// Gets or sets the CVV code.
    /// </summary>
    public string CVV
    {
        get => _cvv;
        set => SetProperty(ref _cvv, value);
    }

    private string _expirationDate;
    /// <summary>
    /// Gets or sets the expiration date of the card.
    /// </summary>
    public string ExpirationDate
    {
        get => _expirationDate;
        set => SetProperty(ref _expirationDate, value);
    }

    private string _amount;
    public string Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _statusMessage;
    /// <summary>
    /// Gets or sets the status message of the payment process.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private string _paymentStatus;
    /// <summary>
    /// Gets or sets the payment status.
    /// </summary>
    public string PaymentStatus
    {
        get => _paymentStatus;
        set => SetProperty(ref _paymentStatus, value);
    }

    private string _message;
    private readonly IUserDAO _userDAO;

    /// <summary>
    /// Gets or sets the message entered by the user.
    /// </summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public PaymentViewModel(string premiumType, string price)
    {
        _userDAO = (App.Current as App).Services.GetRequiredService<IUserDAO>();
        PremiumType = premiumType;
        Amount = price;

        // Set initial selection
        IsStripeSelected = true;
        IsPaypalSelected = false;

        ChooseStripeCommand = new RelayCommand(() =>
        {
            IsStripeSelected = true;
            IsPaypalSelected = false;
        });

        ChoosePaypalCommand = new RelayCommand(() =>
        {
            IsStripeSelected = false;
            IsPaypalSelected = true;
        });

        PayCommand = new AsyncRelayCommand(ProcessPayment);
    }

    /// <summary>
    /// Processes the payment asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ProcessPayment()
    {
        try
        {
            // Validate card details if Stripe is selected
            if (IsStripeSelected && (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CVV) || string.IsNullOrWhiteSpace(ExpirationDate)))
            {
                StatusMessage = "Please provide completed card details.";
                return;
            }

            
            // Prompt user for confirmation
            var dialog = new ContentDialog
            {
                Title = "Confirm Payment",
                Content = "Do you want to proceed with the payment?",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                XamlRoot = App.Current.ShellWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Simulate payment processing (replace with actual payment logic)
                bool paymentSuccess = true; // Replace with real payment status

                if (paymentSuccess)
                {
                    // Update user's premium status
                    var currentUser = (App.Current as App)?.CurrentUser;

                    if (currentUser == null)
                        throw new InvalidOperationException("User not found");

                    currentUser.IsPremium = true;

                    // Update the user's profile in the database
                    await _userDAO.UpdateUserAsync(currentUser.Id, currentUser);

                    // Update Premium
                    var app = App.Current as App;
                    app.IsPremium = true;

                    StatusMessage = "Payment successful! You are now a premium user.";

                    var shellWindow = App.Current.ShellWindow;

                    // Điều hướng đến ArtistPage và truyền success message
                    shellWindow.GetNavigationService().Navigate(typeof(SuccessPage));
                }
                else
                {
                    StatusMessage = "Payment failed. Please try again.";
                }
            }
            else
            {
                StatusMessage = "Payment canceled by user.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Payment failed: {ex.Message}";
        }
    }

    /// <summary>
    /// Extracts the payment intent ID from the client secret.
    /// </summary>
    /// <param name="clientSecret">The client secret.</param>
    /// <returns>The payment intent ID.</returns>
    private string ExtractPaymentIntentId(string clientSecret)
    {
        // Logic to extract Payment Intent ID from the client secret.
        // For Stripe, the clientSecret might look like "pi_1234567890_secret_ABCD1234".
        return clientSecret.Split('_')[1];
    }

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property value and notifies listeners if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="storage">The storage field for the property value.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the property that changed.</param>
    private void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(storage, value))
        {
            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}
