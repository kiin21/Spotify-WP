using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs;

public class PaymentRequestDTO
{
    public string CardNumber { get; set; }
    public string CVV { get; set; }
    public string ExpirationDate { get; set; }
    public decimal Amount { get; set; }
    public string TransactionId { get; set; }
    public string ReturnUrl { get; set; }
    public string Message { get; set; }

}
