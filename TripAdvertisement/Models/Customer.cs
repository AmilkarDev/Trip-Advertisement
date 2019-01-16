using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripAdvertisement.Models
{
    public class Customers
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }


        public string PhoneNum { get; set; }

        public string Role { get; set; }

        public virtual ICollection<Preferences> Preferences { get; set; }
        public virtual ICollection<Locations> Locations { get; set; }
        public virtual ICollection<FeedBacks> FeedBacks { get; set; }
        public virtual ICollection<Tags> Tags { get; set; }

    }
}