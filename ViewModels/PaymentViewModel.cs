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

public class PaymentViewModel : INotifyPropertyChanged
{
    private readonly IPaymentStrategy _stripeStrategy;
    private readonly IPaymentStrategy _paypalStrategy;
    private IPaymentStrategy _currentStrategy;

    private bool _isStripeSelected;
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

    public ICommand ChooseStripeCommand { get; }
    public ICommand ChoosePaypalCommand { get; }
    public ICommand PayCommand { get; }

    private string _cardNumber;
    public string CardNumber
    {
        get => _cardNumber;
        set => SetProperty(ref _cardNumber, value);
    }

    private string _cvv;
    public string CVV
    {
        get => _cvv;
        set => SetProperty(ref _cvv, value);
    }

    private string _expirationDate;
    public string ExpirationDate
    {
        get => _expirationDate;
        set => SetProperty(ref _expirationDate, value);
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _statusMessage;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private string _paymentStatus;
    public string PaymentStatus
    {
        get => _paymentStatus;
        set => SetProperty(ref _paymentStatus, value);
    }

    // New field: Message entered by the user
    private string _message;
    private readonly IUserDAO _userDAO;

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public PaymentViewModel(string premiumType, decimal price)
    {
        _stripeStrategy = new StripeStrategy();
        _paypalStrategy = new PayPalStrategy();
        _currentStrategy = _stripeStrategy;

        Amount = price;

        // Set initial selection
        IsStripeSelected = true;
        IsPaypalSelected = false;

        ChooseStripeCommand = new RelayCommand(() =>
        {
            _currentStrategy = _stripeStrategy;
            IsStripeSelected = true;
            IsPaypalSelected = false;
        });

        ChoosePaypalCommand = new RelayCommand(() =>
        {
            _currentStrategy = _paypalStrategy;
            IsStripeSelected = false;
            IsPaypalSelected = true;
        });

        PayCommand = new AsyncRelayCommand(ProcessPayment);
    }

    private async Task ProcessPayment()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                StatusMessage = "Please enter a message before proceeding with payment.";
                return;
            }

            if (IsStripeSelected && (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CVV) || string.IsNullOrWhiteSpace(ExpirationDate)))
            {
                StatusMessage = "Please provide complete card details.";
                return;
            }

            // Call the ProcessPayment method of the current strategy
            var paymentRequestDTO = new PaymentRequestDTO
            {
                Amount = Amount,
                CardNumber = CardNumber,
                CVV = CVV,
                ExpirationDate = ExpirationDate,
                Message = Message,
                ReturnUrl = "your-return-url",
                TransactionId = Guid.NewGuid().ToString()
            };

            string clientSecret = await _currentStrategy.ProcessPayment(paymentRequestDTO);

            // Extract the Payment Intent ID from the client secret (if available)
            string paymentIntentId = ExtractPaymentIntentId(clientSecret);

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                using var cts = new CancellationTokenSource();
                await PollPaymentStatus(paymentIntentId, cts.Token);
            }
            else
            {
                StatusMessage = "Could not extract Payment Intent ID.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Payment failed: {ex.Message}";
        }
    }

    private async Task PollPaymentStatus(string paymentIntentId, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // Call the GetPaymentStatus method to retrieve the payment status
                PaymentResponseDTO response = await _currentStrategy.GetPaymentStatus(paymentIntentId);

                // Update the PaymentStatus property with the current status
                PaymentStatus = $"Status: {response.PaymentStatus}";
                OnPropertyChanged(nameof(PaymentStatus));

                // Stop polling if the payment has succeeded or failed
                if (response.PaymentStatus == "succeeded" || response.PaymentStatus == "failed")
                {
                    StatusMessage = response.PaymentStatus == "succeeded"
                        ? "Payment successful!"
                        : "Payment failed.";

                    if (response.PaymentStatus == "succeeded")
                    {
                        // change the isPremium field in the user's profile to true
                        StatusMessage = "Payment successful! You are now a premium user.";

                        var currentUser = (App.Current as App).CurrentUser;

                        if (currentUser == null)
                            throw new InvalidOperationException("User not found");

                        currentUser.IsPremium = true;

                        // Update the user's profile in the database
                        await _userDAO.UpdateUserAsync(currentUser.Id, currentUser);

                    }

                    break; // Exit the polling loop
                }
            }
            catch (Exception ex)
            {
                PaymentStatus = "Error while polling: " + ex.Message;
                OnPropertyChanged(nameof(PaymentStatus));
                break;
            }

            // Wait for 5 seconds before the next poll
            await Task.Delay(5000, token);
        }
    }

    private string ExtractPaymentIntentId(string clientSecret)
    {
        // Logic to extract Payment Intent ID from the client secret.
        // For Stripe, the clientSecret might look like "pi_1234567890_secret_ABCD1234".
        return clientSecret.Split('_')[1];
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(storage, value))
        {
            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}

