using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spotify.Services;

public class StripeStrategy : IPaymentStrategy
{
    public async Task<string> ProcessPayment(PaymentRequestDTO paymentRequestDTO)
    {
        // Retrieve the API key from environment variables
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        // Validate Card Details
        ValidateCardDetails(paymentRequestDTO);

        // Create a Payment Method
        var paymentMethod = await CreatePaymentMethod(paymentRequestDTO);

        // Create Payment Intent
        var paymentIntent = await CreatePaymentIntent(paymentRequestDTO, paymentMethod.Id);

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

        // Convert expiration date to month/year
        var expirationDateParts = paymentRequestDTO.ExpirationDate.Split('/');
        if (expirationDateParts.Length != 2 ||
            !int.TryParse(expirationDateParts[0], out int expirationMonth) ||
            !int.TryParse(expirationDateParts[1], out int expirationYear))
        {
            throw new ArgumentException("Invalid expiration date format.");
        }
    }

    private async Task<PaymentMethod> CreatePaymentMethod(PaymentRequestDTO paymentRequestDTO)
    {
        // Convert expiration date to month/year
        var expirationDateParts = paymentRequestDTO.ExpirationDate.Split('/');
        int expirationMonth = int.Parse(expirationDateParts[0]);
        int expirationYear = int.Parse(expirationDateParts[1]);

        var paymentMethodOptions = new PaymentMethodCreateOptions
        {
            Type = "card", // Add the type here to specify it's a card payment method
            Card = new PaymentMethodCardOptions
            {
                Number = paymentRequestDTO.CardNumber,
                ExpMonth = expirationMonth,
                ExpYear = expirationYear,
                Cvc = paymentRequestDTO.CVV
            }
        };

        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(paymentMethodOptions);
        return paymentMethod;
    }


    private async Task<PaymentIntent> CreatePaymentIntent(PaymentRequestDTO paymentRequestDTO, string paymentMethodId)
    {
        var paymentIntentOptions = new PaymentIntentCreateOptions
        {
            Amount = (long)(paymentRequestDTO.Amount * 100), // Convert to cents
            Currency = "usd",
            PaymentMethod = paymentMethodId,
            ConfirmationMethod = "manual",
            Confirm = true
        };

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);
        return paymentIntent;
    }

    public async Task<PaymentResponseDTO> GetPaymentStatus(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var paymentIntent = await service.GetAsync(paymentIntentId);

        return new PaymentResponseDTO
        {
            PaymentId = paymentIntent.Id,
            PaymentStatus = paymentIntent.Status,
            PaymentAmount = paymentIntent.Amount / 100,
            PaymentCurrency = paymentIntent.Currency,
            PaymentMethod = paymentIntent.PaymentMethod?.ToString(),
        };
    }
}
