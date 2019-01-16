using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TripAdvertisement.Models
{
   public  class Tags
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string InterestCount { get; set; }
        public virtual ICollection<Locations> Locations { get; set; }
        public virtual ICollection<Customers> Customers { get; set; }
    }
}
