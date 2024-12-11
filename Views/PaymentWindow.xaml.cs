//using Microsoft.UI.Xaml.Controls;
//using Microsoft.Web.WebView2.Core;
//using Spotify.Contracts.DAO;
//using Spotify.Services;
//using System;
//using System.Diagnostics;
//using System.Text.Json;
//using System.Threading.Tasks;
//using System.Windows;
//using System.IO;
//using Spotify.Models.DTOs;

//namespace Spotify.Views;

//public partial class PaymentWindow: Microsoft.UI.Xaml.Window
//{
////    private WebView2 _webView;
//    private readonly string _stripePublishableKey;
//    private readonly decimal _amount;
//    private readonly IUserDAO _userDAO;
//    private readonly StripeStrategy _stripeStrategy;

//    public PaymentWindow(decimal amount, IUserDAO userDAO)
//    {
//        _userDAO = userDAO;
//        _amount = amount;
//        _stripePublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
//        Debug.WriteLine($"Stripe Publishable Key: {_stripePublishableKey}");

//        // Initialize the window's content with a new Grid
//        //    var grid = new Grid();
//        //    Content = grid;

//        _stripeStrategy = new StripeStrategy();
//        this.InitializeComponent();
//        InitializeWebView(); // Pass the grid to InitializeWebView
//    }

//    private async void InitializeWebView()
//    {
//        try
//        {
//            //_webView = new WebView2
//            //{
//            //    Width = ActualWidth,
//            //    Height = ActualHeight
//            //};

//            //grid.Children.Add(_webView);

//            // More explicit environment initialization
//            var env = await CoreWebView2Environment.CreateAsync();
//            //    await _webView.EnsureCoreWebView2Async(env);
//            await MyWebView.EnsureCoreWebView2Async(env);

//            // Configure WebView to allow local resource loading if needed
//            //_webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
//            //_webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

//            // Configure WebView to allow local resource loading if needed
//            MyWebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
//            MyWebView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

//            MyWebView.CoreWebView2.WebMessageReceived += async (sender, args) =>
//            {
//                try
//                {
//                    var message = JsonSerializer.Deserialize<dynamic>(args.WebMessageAsJson);

//                    if (message.type == "createPaymentIntent")
//                    {
//                        // Create payment intent locally
//                        var paymentRequest = new PaymentRequestDTO
//                        {
//                            Amount = (decimal)message.amount
//                        };

//                        var paymentIntent = await _stripeStrategy.CreatePaymentIntent(paymentRequest);

//                        // Send back the client secret
//                        MyWebView.CoreWebView2.PostWebMessageAsJson(new
//                        {
//                            clientSecret = paymentIntent.ClientSecret
//                        }.ToString());
//                    }
//                }
//                catch (Exception ex)
//                {
//                    // Handle error
//                    MyWebView.CoreWebView2.PostWebMessageAsJson(new
//                    {
//                        error = ex.Message
//                    }.ToString());
//                }
//            };
       

//            // Generate and load the payment page HTML content
//            string htmlContent = GenerateStripePaymentHtml();
//            //    _webView.NavigateToString(htmlContent);
//            MyWebView.NavigateToString(htmlContent);
//        }
//        catch (Exception ex)
//        {
//            // Log the detailed error and show a user-friendly message
//           MessageBox.Show(ex.Message );
//        }
//    }

//    private string GenerateStripePaymentHtml()
//    {
//        string htmlTemplate = @"
//        <!DOCTYPE html>
//        <html>
//        <head>
//            <script src='https://js.stripe.com/v3/'></script>
//        </head>
//        <body>
//            <form id='payment-form'>
//                <div id='card-element'></div>
//                <button id='submit'>Pay {AMOUNT}</button>
//            </form>

//            <script>
//                var stripe = Stripe('{PUBLISHABLE_KEY}');
//                var elements = stripe.elements();
//                var cardElement = elements.create('card');
//                cardElement.mount('#card-element');

//                var form = document.getElementById('payment-form');
//                form.addEventListener('submit', async (event) => {
//                    event.preventDefault();

//                    try {
//                        // Use a method to create payment intent locally
//                        const response = await window.chrome.webview.postMessageWithResponse('createPaymentIntent', {
//                            amount: {AMOUNT}
//                        });

//                        const { clientSecret } = response;

//                        const result = await stripe.confirmCardPayment(clientSecret, {
//                            payment_method: {
//                                card: cardElement
//                            }
//                        });

//                        if (result.paymentIntent.status === 'succeeded') {
//                            window.chrome.webview.postMessage(JSON.stringify({
//                                type: 'paymentSuccess',
//                                paymentIntentId: result.paymentIntent.id
//                            }));
//                        }
//                    } catch (error) {
//                        window.chrome.webview.postMessage(JSON.stringify({
//                            type: 'paymentError',
//                            message: error.message
//                        }));
//                    }
//                });
//            </script>
//        </body>
//        </html>";

//        // Replace placeholders
//        string htmlContent = htmlTemplate
//            .Replace("{PUBLISHABLE_KEY}", _stripePublishableKey)
//            .Replace("{AMOUNT}", _amount.ToString());

//        return htmlContent;
//    }

//    //private string GenerateStripePaymentHtml()
//    //{
//    //    //// Get the base directory where the executable runs
//    //    //string baseDirectory = AppContext.BaseDirectory;

//    //    //// Combine with the relative path to the Assets folder
//    //    string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
//    ////    string assetsPath = Path.Combine(currentDirectory, "Assets", "stripe_payment.html");
//    ////    string assetsPath = Path.Combine(baseDirectory, "Assets", "stripe_payment.html");
//    //    string assetsPath = "D:\\KHTN\\Spotify Window C#\\Spotify\\Spotify-WP\\Assets\\stripe_payment.html";

//    //    //string assetsPath = "stripe_payment.html";

//    //    // Log or debug the path for verification
//    //    Debug.WriteLine($"Assets Path: {assetsPath}");

//    //    // Ensure the file exists at the specified path
//    //    if (!File.Exists(assetsPath))
//    //    {
//    //        throw new FileNotFoundException($"The specified file was not found: {assetsPath}");
//    //    }

//    //    // Read the HTML template from the resolved path
//    //    string htmlTemplate = File.ReadAllText(assetsPath);

//    //    // Replace placeholders with actual values
//    //    string htmlContent = htmlTemplate
//    //        .Replace("{PUBLISHABLE_KEY}", _stripePublishableKey)
//    //        .Replace("{AMOUNT}", System.Web.HttpUtility.HtmlEncode(_amount.ToString("C"))); // Escape amount to prevent XSS

//    //    return htmlContent;
//    //}

//    //private string GenerateStripePaymentHtml()
//    //{
//    //    // Embedded HTML content
//    //    string htmlTemplate = @"
//    //    <!DOCTYPE html>
//    //    <html>
//    //    <head>
//    //        <title>Stripe Payment</title>
//    //        <script src='https://js.stripe.com/v3/'></script>
//    //        <style>
//    //            body { 
//    //                font-family: Arial, sans-serif; 
//    //                display: flex; 
//    //                justify-content: center; 
//    //                align-items: center; 
//    //                height: 100vh; 
//    //                margin: 0; 
//    //                background-color: #f0f0f0; 
//    //            }
//    //            #payment-form { 
//    //                width: 400px; 
//    //                padding: 20px; 
//    //                background-color: white; 
//    //                border-radius: 8px; 
//    //                box-shadow: 0 4px 6px rgba(0,0,0,0.1); 
//    //            }
//    //            .form-row { 
//    //                margin-bottom: 15px; 
//    //            }
//    //            .form-row label { 
//    //                display: block; 
//    //                margin-bottom: 5px; 
//    //                font-weight: bold; 
//    //            }
//    //            .form-row input, 
//    //            .form-row #card-element { 
//    //                width: 100%; 
//    //                padding: 10px; 
//    //                border: 1px solid #ccc; 
//    //                border-radius: 4px; 
//    //            }
//    //            #submit { 
//    //                width: 100%; 
//    //                padding: 10px; 
//    //                background-color: #007bff; 
//    //                color: white; 
//    //                border: none; 
//    //                border-radius: 4px; 
//    //                cursor: pointer; 
//    //            }
//    //            #card-errors {
//    //                color: red;
//    //                margin-top: 10px;
//    //                text-align: center;
//    //            }
//    //        </style>
//    //    </head>
//    //    <body>
//    //        <form id='payment-form'>
//    //            <div class='form-row'>
//    //                <label for='card-element'>Credit or debit card</label>
//    //                <div id='card-element'></div>
//    //                <div id='card-errors' role='alert'></div>
//    //            </div>
//    //               <div class='form-row'>
//    //         <label for='card-name'>Card Number</label>
//    //         <input id='card-name' type='text' placeholder='4242 4242  4242' required>
//    //     </div>
//    //     <div class='form-row'>
//    //        <label for='card-name'>CVV</label>
//    //        <input id='card-name' type='text' placeholder='234' required>
//    //    </div>
//    //            <button id='submit'>Pay {AMOUNT}</button>
//    //        </form>

//    //        <script>
//    //            var stripe = Stripe('{PUBLISHABLE_KEY}');
//    //            var elements = stripe.elements();

//    //            var style = {
//    //                base: {
//    //                    color: '#32325d',
//    //                    fontFamily: 'Helvetica Neue, Helvetica, sans-serif',
//    //                    fontSmoothing: 'antialiased',
//    //                    fontSize: '16px',
//    //                    '::placeholder': {
//    //                        color: '#aab7c4'
//    //                    }
//    //                },
//    //                invalid: {
//    //                    color: '#fa755a',
//    //                    iconColor: '#fa755a'
//    //                }
//    //            };

//    //            var cardElement = elements.create('card', { style: style });
//    //            cardElement.mount('#card-element');

//    //            cardElement.addEventListener('change', function(event) {
//    //                var displayError = document.getElementById('card-errors');
//    //                if (event.error) {
//    //                    displayError.textContent = event.error.message;
//    //                } else {
//    //                    displayError.textContent = 'Payment Success!!!';
//    //                }
//    //            });

//    //            var form = document.getElementById('payment-form');
//    //            form.addEventListener('submit', async (event) => {
//    //                event.preventDefault();

//    //                try {
//    //                    const response = await fetch('/create-payment-intent', {
//    //                        method: 'POST',
//    //                        headers: { 'Content-Type': 'application/json' },
//    //                        body: JSON.stringify({ amount: {AMOUNT} })
//    //                    });

//    //                    const { clientSecret } = await response.json();

//    //                    const result = await stripe.confirmCardPayment(clientSecret, {
//    //                        payment_method: {
//    //                            card: cardElement,
//    //                            billing_details: {
//    //                                name: 'Customer Name'
//    //                            }
//    //                        }
//    //                    });

//    //                    if (result.error) {
//    //                        var errorElement = document.getElementById('card-errors');
//    //                        errorElement.textContent = result.error.message;

//    //                        window.chrome.webview.postMessage(JSON.stringify({
//    //                            type: 'paymentError',
//    //                            message: result.error.message
//    //                        }));
//    //                    } else if (result.paymentIntent.status === 'succeeded') {
//    //                        window.chrome.webview.postMessage(JSON.stringify({
//    //                            type: 'paymentSuccess',
//    //                            paymentIntentId: result.paymentIntent.id
//    //                        }));
//    //                    }
//    //                } catch (error) {
//    //                    var errorElement = document.getElementById('card-errors');
//    //                    errorElement.textContent = error.message;

//    //                    window.chrome.webview.postMessage(JSON.stringify({
//    //                        type: 'paymentError',
//    //                        message: error.message
//    //                    }));
//    //                }
//    //            });
//    //        </script>
//    //    </body>
//    //    </html>";

//    //    // Replace placeholders with actual values
//    //    string htmlContent = htmlTemplate
//    //        .Replace("{PUBLISHABLE_KEY}", _stripePublishableKey)
//    //        .Replace("{AMOUNT}", System.Web.HttpUtility.HtmlEncode(_amount.ToString("C"))); // Escape amount to prevent XSS

//    //    return htmlContent;
//    //}

//    private async void WebView_WebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
//    {
//        try
//        {
//            var message = JsonSerializer.Deserialize<PaymentMessage>(args.WebMessageAsJson);

//            if (message.Type == "paymentSuccess")
//            {
//                // Verify payment and update user status
//                await VerifyPaymentAndUpdateUser(message.PaymentIntentId);

//             //   Close(); // Close payment window
//            }
//            else if (message.Type == "paymentError")
//            {
//                // Show error message
//                MessageBox.Show("Payment failed. Please try again.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        catch (Exception ex)
//        {
//            // Log error and handle appropriately
//            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }

//    private async Task VerifyPaymentAndUpdateUser(string paymentIntentId)
//    {
//        // Verify payment status and update user premium status
//        var paymentStrategy = new StripeStrategy();
//        var paymentResponse = await paymentStrategy.GetPaymentStatus(paymentIntentId);

//        if (paymentResponse.PaymentStatus == "succeeded")
//        {
//            // Update current user's premium status
//            var currentUser = (App.Current as App).CurrentUser;
//            currentUser.IsPremium = true;
//            await _userDAO.UpdateUserAsync(currentUser.Id, currentUser);
//            Console.WriteLine("Payment Success");
//            //Close();
//        }
//    }

//}

//// Helper class to handle WebView messages
//public class PaymentMessage
//{
//    public string Type { get; set; }
//    public string PaymentIntentId { get; set; }
//    public string Message { get; set; }
//}