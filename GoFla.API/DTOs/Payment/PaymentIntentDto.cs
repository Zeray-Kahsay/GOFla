using System;

namespace GoFla.API.DTOs.Payment;

public class PaymentIntentDto ( string IntentId, string ClientSecret )
{
    public string  IntentId  { get; set; } = IntentId;
    public string  ClientSecret { get; set; } = ClientSecret;


    

}
