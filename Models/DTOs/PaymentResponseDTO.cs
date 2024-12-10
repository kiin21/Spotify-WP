using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs;

public class PaymentResponseDTO
{
    public string PaymentId { get; set; }
    public string PaymentStatus { get; set; }
    public decimal PaymentAmount { get; set; }
    public string PaymentCurrency { get; set; }
    public string PaymentMethod { get; set; }
}
