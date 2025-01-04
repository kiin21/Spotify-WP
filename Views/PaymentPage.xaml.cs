using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.ViewModels;
using System;

namespace Spotify.Views;

/// <summary>
/// A page that handles payment processing.
/// </summary>
public sealed partial class PaymentPage : Page
{
    /// <summary>
    /// Gets the view model for the payment page.
    /// </summary>
    public PaymentViewModel ViewModel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentPage"/> class.
    /// </summary>
    public PaymentPage()
    {
        this.InitializeComponent();
        ViewModel = new PaymentViewModel("", "0");  // Initialize with default values
        DataContext = ViewModel;  // Set DataContext here
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
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

