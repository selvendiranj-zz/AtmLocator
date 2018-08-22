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
}
