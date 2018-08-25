using System;
using System.Collections.Generic;
using System.Text;

namespace AtmLocator.Fass
{
    public class AtmLocation
    {
        public AtmAddress Address { get; set; }
        public string Distance { get; set; }
        public string Type { get; set; }
    }

    public class AtmAddress
    {
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }

    public class GeoLocation
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class AtmSimplified
    {
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
    }
}
