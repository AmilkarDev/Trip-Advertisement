using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TripAdvertisement.Models
{
    public class Images
    {
        public int Id { get; set; }

        public string Link { get; set; }
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }

        public int LocationId { get; set; }

           [ForeignKey("LocationId")]
        public virtual Locations Locations { get; set; }
    }
}
