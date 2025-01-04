using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;
using Spotify.Services;
using Stripe;
using System;
using System.Diagnostics;

namespace Spotify.Views;

/// <summary>
/// A page that handles premium subscription options.
/// </summary>
public sealed partial class PremiumPage : Page
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    private static UserDTO CurrentUser => App.Current.CurrentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="PremiumPage"/> class.
    /// </summary>
    public PremiumPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the click event for the Premium Mini subscription button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnGetPremiumMiniClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Subscription",
            Content = "You selected Premium Mini. Proceed with payment?",
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Proceed",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Navigate to PaymentPage with parameters
            NavigateToPaymentPage("Premium Mini", "10.500 VND"); // Example price
        }
    }

    /// <summary>
    /// Handles the click event for the Premium Individual subscription button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnGetPremiumIndividualClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Subscription",
            Content = "You selected Premium Individual. Proceed with payment?",
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Proceed",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Navigate to PaymentPage with parameters
            NavigateToPaymentPage("Premium Individual", "29.500 VND"); // Example price
        }
    }

    /// <summary>
    /// Navigates to the payment page with the specified premium type and price.
    /// </summary>
    /// <param name="premiumType">The type of premium subscription.</param>
    /// <param name="price">The price of the premium subscription.</param>
    private void NavigateToPaymentPage(string premiumType, string price)
    {
        var shellWindow = App.Current.ShellWindow;

        if (!CurrentUser.IsPremium)
        {
            shellWindow.GetNavigationService().Navigate(typeof(PaymentPage), (premiumType, price));
        }
        else
        {
            shellWindow.GetNavigationService().Navigate(typeof(SuccessPage));
        }
    }
}
