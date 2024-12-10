using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Spotify.Views;

public sealed partial class PremiumPage : Page
{
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
            NavigateToPaymentPage("Premium Mini", 4.99m); // Example price
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
            NavigateToPaymentPage("Premium Individual", 9.99m); // Example price
        }
    }

    private void NavigateToPaymentPage(string premiumType, decimal price)
    {
        var shellWindow = App.Current.ShellWindow;

        shellWindow.GetNavigationService().Navigate(typeof(PaymentPage), (premiumType, price));
    }
}
