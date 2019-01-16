using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TripAdvertisement.Models
{
   public  class Locations
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageLink { get; set; }

        public string Address { get; set; }

        public string Area { get; set; }

        public string Latitude { get; set; }

        public string Logtitude { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string AverageTime { get; set; }
        public Customers customer { get; set; }

        public virtual ICollection<Images> Images { get; set; }
        public virtual ICollection<Tags> Tags { get; set; }
    }
}
