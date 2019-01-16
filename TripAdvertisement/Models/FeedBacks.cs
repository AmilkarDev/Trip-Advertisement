using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TripAdvertisement.Models
{
    public class FeedBacks
    {
        public int Id { get; set; }

        public string Date { get; set; }

        public string About { get; set; }

        public string Description { get; set; }

        public int Customer_Id { get; set; }

           [ForeignKey("Customer_Id")]
        public virtual Customers Customers { get; set; }
    }
}
