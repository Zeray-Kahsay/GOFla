using System;

namespace GoFla.API.Domain;

public class DeliveryZone
{
    public int  Id  { get; set; }
    public string  CountryCode  { get; set; } = string.Empty;
    public string  PostalCodePattern  { get; set; } = string.Empty;
    public bool  IsActive  { get; set; } = true;
}
