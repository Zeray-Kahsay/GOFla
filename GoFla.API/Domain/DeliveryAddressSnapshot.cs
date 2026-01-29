using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFla.API.Domain
{
    public class DeliveryAddressSnapshot
    {
        public string  Street  { get; set; } = string.Empty;
        public string  City  { get; set; } = string.Empty;
        public string  PostalCode  { get; set; } = string.Empty;
        public string  CountryCode  { get; set; } = string.Empty;

        public double? Latitude  { get; set; }
        public double? Longitude { get; set; }
    }
}