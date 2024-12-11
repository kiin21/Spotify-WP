using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.ViewModels;
using System;

namespace Spotify.Views;

public sealed partial class PaymentPage: Page
{
    public PaymentViewModel ViewModel { get; private set; }

    public PaymentPage()
    {
        this.InitializeComponent();
        ViewModel = new PaymentViewModel("", "0");  // Initialize with default values
        DataContext = ViewModel;  // Set DataContext here
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // If e.Parameter contains valid data, update ViewModel
        if (e.Parameter is (string premiumType, string price))
        {
            ViewModel.PremiumType = premiumType;  // Update properties if needed
            ViewModel.Amount = price;
        }
    }
}
