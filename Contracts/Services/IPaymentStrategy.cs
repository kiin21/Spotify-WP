using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.Services;

public interface IPaymentStrategy
{
    Task<PaymentResponseDTO> GetPaymentStatus(string paymentIntentId);
    public Task<string> ProcessPayment(PaymentRequestDTO paymentRequestDTO);
}

