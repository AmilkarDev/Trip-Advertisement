using TripAdvertisement.MVC5.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TripAdvertisement.Models
{

    public class EditUserViewModel
        {
            public string Id { get; set; }

            [Required(AllowEmptyStrings = false)]
            [Display(Name = "Email")]
            [EmailAddress]
            public string Email { get; set; }

            // Add the Address Info:
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            // Use a sensible display name for views:
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            public string PhoneNumber { get; set; }
            public string UserName { get; set; }
            public IEnumerable<SelectListItem> RolesList { get; set; }
        }

        public class editLocation
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
            public IEnumerable<SelectListItem> tags { get; set; }
        }

    public class editCustomer
    {

        public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }


        public string PhoneNum { get; set; }
        public ICollection<Preferences> Preferences { get; set; }
        public string Role { get; set; }
        public IEnumerable<SelectListItem> tags { get; set; }
    }
    
 
    }
