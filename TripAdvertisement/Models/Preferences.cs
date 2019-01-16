using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripAdvertisement.Models
{
    public class Preferences
    {
        public int id { get; set; }
        public string Country { get; set; }
        public string city { get; set; }
        public string Tag { get; set; }
        public Customers customer { get; set; }
    }
}