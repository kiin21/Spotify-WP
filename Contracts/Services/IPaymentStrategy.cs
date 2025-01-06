using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.Services;

/// <summary>
/// Interface for payment strategy
/// </summary>
public interface IPaymentStrategy
{
    /// <summary>
    /// Retrieves the payment status for a given payment intent ID.
    /// </summary>
    /// <param name="paymentIntentId">The ID of the payment intent.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a PaymentResponseDTO object.</returns>
    Task<PaymentResponseDTO> GetPaymentStatus(string paymentIntentId);

    /// <summary>
    /// Processes a payment request.
    /// </summary>
    /// <param name="paymentRequestDTO">The payment request data transfer object.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the payment processing result as a string.</returns>
    Task<string> ProcessPayment(PaymentRequestDTO paymentRequestDTO);
}


