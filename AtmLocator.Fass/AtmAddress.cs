using System;
using System.Collections.Generic;
using System.Text;

namespace AtmLocator.Fass
{
    public class AtmAddress
    {
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }
}
