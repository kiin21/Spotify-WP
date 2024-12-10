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
    }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is (string premiumType, decimal price))
        {
            ViewModel = new PaymentViewModel(premiumType, price);
            DataContext = ViewModel;
        }
    }
}
