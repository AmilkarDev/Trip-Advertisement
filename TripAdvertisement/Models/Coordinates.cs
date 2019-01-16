using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripAdvertisement.Models
{
    public class Coordinates
    {
        public string Id { get; set; }
        //public float Altitude { get; set; }
        //public float Longtitude { get; set; }
        public string Altitude { get; set; }
        public string Longtitude { get; set; }
        public string  Name { get; set; }
        public string Address { get; set; }
        public string  PhoneNum { get; set; }
        public string InternationalPhoneNum { get; set; }
        public string Rating { get; set; }
        public bool OpenNow { get; set; }
        public string IconUrl { get; set; }
        public List<string> PhotoUrls { get; set; }
        public List<string> Types { get; set; }

    }
}