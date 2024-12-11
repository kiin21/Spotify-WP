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

public sealed partial class PremiumPage : Page
{
    private static UserDTO CurrentUser => App.Current.CurrentUser;

    public PremiumPage()
    {
        this.InitializeComponent();
    }

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
       
  
        
        

          
    //    var paymentWindow = new PaymentWindow(price, (App.Current as App).Services.GetRequiredService<IUserDAO>());
    //    paymentWindow.Show();

    
