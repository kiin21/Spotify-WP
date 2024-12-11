using Spotify.Models.DTOs;
using Stripe;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Spotify.Contracts.Services;

namespace Spotify.Services
{
    public class StripeStrategy
    {
        public async Task<string> ProcessPayment(PaymentRequestDTO paymentRequestDTO)
        {
            // Retrieve the API key from environment variables
            string apiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Stripe API key is not configured.");
            }

            StripeConfiguration.ApiKey = apiKey;

            // Validate card details
            ValidateCardDetails(paymentRequestDTO);

            // Create a Payment Method
            PaymentMethod paymentMethod = await CreatePaymentMethodAsync(paymentRequestDTO);

            // Create Payment Intent
            PaymentIntent paymentIntent = await CreatePaymentIntentAsync(paymentRequestDTO, paymentMethod.Id);

            // Return client secret for further payment confirmation
            return paymentIntent.ClientSecret;
        }

        private void ValidateCardDetails(PaymentRequestDTO paymentRequestDTO)
        {
            if (string.IsNullOrWhiteSpace(paymentRequestDTO.CardNumber) ||
                string.IsNullOrWhiteSpace(paymentRequestDTO.CVV) ||
                string.IsNullOrWhiteSpace(paymentRequestDTO.ExpirationDate))
            {
                throw new ArgumentException("Please provide complete card details.");
            }

            // Check expiration date format (MM/YYYY)
            var expirationDateParts = paymentRequestDTO.ExpirationDate.Split('/');
            if (expirationDateParts.Length != 2 ||
                !int.TryParse(expirationDateParts[0], out int expirationMonth) ||
                !int.TryParse(expirationDateParts[1], out int expirationYear) ||
                expirationMonth < 1 || expirationMonth > 12)
            {
                throw new ArgumentException("Invalid expiration date format. Use MM/YYYY.");
            }

            // Optional: Check if the card is expired
            var currentDate = DateTime.UtcNow;
            if (expirationYear < currentDate.Year ||
                (expirationYear == currentDate.Year && expirationMonth < currentDate.Month))
            {
                throw new ArgumentException("Card has expired.");
            }
        }

        private async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentRequestDTO paymentRequestDTO)
        {
            // Extract expiration date parts
            var expirationDateParts = paymentRequestDTO.ExpirationDate.Split('/');
            int expirationMonth = int.Parse(expirationDateParts[0]);
            int expirationYear = int.Parse(expirationDateParts[1]);

            // Create a payment method
            var paymentMethodOptions = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = paymentRequestDTO.CardNumber,
                    ExpMonth = expirationMonth,
                    ExpYear = expirationYear,
                    Cvc = paymentRequestDTO.CVV,
                }
            };

            var paymentMethodService = new PaymentMethodService();
            return await paymentMethodService.CreateAsync(paymentMethodOptions);
        }

        private async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO paymentRequestDTO, string paymentMethodId)
        {
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(paymentRequestDTO.Amount * 100), // Convert dollars to cents
                Currency = "usd",
                PaymentMethod = paymentMethodId,
                ConfirmationMethod = "manual", // Use manual confirmation to handle actions like 3D Secure
                Confirm = true
            };

            var paymentIntentService = new PaymentIntentService();
            return await paymentIntentService.CreateAsync(paymentIntentOptions);
        }

        public async Task<PaymentResponseDTO> GetPaymentStatus(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            return new PaymentResponseDTO
            {
            //    PaymentId = paymentIntent.Id,
                PaymentStatus = paymentIntent.Status,
            //    PaymentAmount = paymentIntent.Amount / 100.0m, // Convert back to dollars
            //    PaymentCurrency = paymentIntent.Currency,
            //    PaymentMethod = paymentIntent.PaymentMethod?.ToString(),
            };
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Spotify.Contracts.Services;
//using Spotify.Models.DTOs;
//using Stripe;
//using System;
//using System.Runtime.InteropServices;
//using System.Threading.Tasks;
//using Windows.ApplicationModel.Payments;

//namespace Spotify.Services;

//public class StripeStrategy
//{
//    private readonly string _stripeSecretKey;

//    public StripeStrategy()
//    {
//        _stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"); // Your Stripe Secret Key
//        StripeConfiguration.ApiKey = _stripeSecretKey;
//    }

//    /// <summary>
//    /// Retrieves the payment status for a given PaymentIntent ID from Stripe.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to retrieve.</param>
//    /// <returns>The status of the payment.</returns>
//    public async Task<PaymentResponseDTO> GetPaymentStatus(string paymentIntentId)
//    {
//        var service = new PaymentIntentService();
//        PaymentIntent paymentIntent = await service.GetAsync(paymentIntentId);

//        var paymentResponseDTO = new PaymentResponseDTO
//        {
//            PaymentStatus = paymentIntent.Status
//        };

//        return paymentResponseDTO;
//    }


//    public async Task<PaymentIntent> CreatePaymentIntent(PaymentRequestDTO paymentRequest)
//    {
//        try
//        {
//            var options = new PaymentIntentCreateOptions
//            {
//                Amount = (long?)(paymentRequest.Amount * 100),
//                Currency = "usd",
//                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
//                {
//                    Enabled = true,
//                }
//            };

//            var service = new PaymentIntentService();
//            var paymentIntent = await service.CreateAsync(options);
//            paymentIntent
//            return;
//        }
//        catch (StripeException ex)
//        {
//            // Log or handle the exception
//            throw;
//        }
//    }


//    // StripeController to handle API requests related to payments
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StripeController : ControllerBase
//    {
//        private readonly string _stripeSecretKey;

//        public StripeController()
//        {
//            _stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"); // Your Stripe Secret Key
//            StripeConfiguration.ApiKey = _stripeSecretKey;
//        }

//        [HttpPost("create-payment-intent")]
//        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequestDTO paymentRequest)
//        {
//            try
//            {
//                var options = new PaymentIntentCreateOptions
//                {
//                    Amount = (long?)(paymentRequest.Amount * 100), // Amount in cents
//                    Currency = "usd", // Set the currency here
//                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
//                    {
//                        Enabled = true,
//                    },
//                };

//                var service = new PaymentIntentService();
//                var paymentIntent = await service.CreateAsync(options);

//                return Ok(new { clientSecret = paymentIntent.ClientSecret });
//            }
//            catch (StripeException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }
//    }
//}



