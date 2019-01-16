using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Threading.Tasks;
using TripAdvertisement.Models;
using System.Linq;
using TripAdvertisement.DAL;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Newtonsoft.Json;
using System.Xml;
using System.Collections.Specialized;
using System.Data.Entity.Spatial;
using HtmlAgilityPack;

namespace TripAdvertisement.Controllers
{
    // Edit and details of the current user profile / tags / preferences 
    [Authorize]
    public class MyProfileController : Controller
    {
        ApplicationDbContext adb;
        TripAdvertisementContext db;
        public MyProfileController() {
            db = new TripAdvertisementContext();
            adb = new ApplicationDbContext();
        }

        public MyProfileController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
          UserManager = userManager;
            RoleManager = roleManager;

        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: MyProfile
        public async Task<ActionResult> Index()
        {
            //String UserIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(UserIP))
            //{
            //    UserIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}

            var response = new WebClient().DownloadString("http://checkip.dyndns.org/");

            int index = response.IndexOf(':');
            int index2 = response.IndexOf('<', index);
            string UserIP = response.Substring(index+2, index2 - (index+2));


            // string url = "http://freegeoip.net/json/" + UserIP;
            //string url = "https://ipinfo.io/" + UserIP + "/geo";
            string url = "https://ipinfo.io/json";
            WebClient client = new WebClient();
            string jsonstring = client.DownloadString(url);
            dynamic dynObj = JsonConvert.DeserializeObject(jsonstring);

            System.Web.HttpContext.Current.Session["UserCountryCode"] = dynObj.country_code;


            const string point1text = "POINT(6.3692109 53.1459672)";

            const string point2text = "POINT(6.3924875 53.1625221)";
            DbGeography point1 = DbGeography.FromText(point1text);
            DbGeography point2 = DbGeography.FromText(point2text);
            var distance = point1.Distance(point2);

           

       
            





        string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();

            /* Temporary code to add the already registered users to the customer table
             *  , after that every user will be added automatically when registering */
            if (cust == null)
            {
                cust = new Customers {FullName=user.UserName, UserName = user.UserName, Email = user.Email, PhoneNum = user.PhoneNumber };
                db.Customers.Add(cust);
                cust.Locations = new List<Locations>();
                cust.Tags = new List<Tags>();
                cust.Preferences = new List<Preferences>();
                db.SaveChanges();
            }
            Session["Customer"]= cust;
            List<Tags> tags = new List<Tags>();
                tags = cust.Tags.ToList();
                List<string> tagsNames = new List<string>();
                foreach (var item in tags)
                {
                    tagsNames.Add(item.Name);
                }
             ViewBag.tags = tagsNames;

            var TagsList = db.Tags.ToList().Select(x => new SelectListItem()
            {
                Selected = tagsNames.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });






            List<string> countries = new List<string>();
            countries.Add("Select");
            List<string> cities = new List<string>();
            cities.Add("Select");
            List<string> taggs = new List<string>();
            taggs.Add("Select");
            foreach (var item in db.Locations.ToList())
            {
                string country = item.Country;
                countries.Add(country);
                string city = item.City;
                cities.Add(city);
            }
            foreach (var item in db.Tags.ToList())
            {
                string tt = item.Name;
                taggs.Add(tt);
            }
            countries = countries.Distinct().ToList();
            cities = cities.Distinct().ToList();
            SelectList mycountries = new SelectList(countries);
            SelectList mycities = new SelectList(cities);
            SelectList mytags = new SelectList(taggs);
            ViewBag.listCountry = mycountries;
            Session["mycountries"] = mycountries;
            ViewBag.listCity = mycities;
            Session["mycities"] = mycities;
            ViewBag.taggs = mytags;
            Session["mytags"] = mytags;





            List<Preferences> list = new List<Preferences>();
            foreach (var item in cust.Preferences.ToList())
            {
                list.Add(item);
            }
            ViewBag.prefs = list;


            //  ViewBag.tagsList = TagsList;
            return View(new editCustomer
            {
                FullName = cust.FullName,
                Email = cust.Email,
                PhoneNum = cust.PhoneNum,
                Id = cust.Id,
                UserName = cust.UserName,
                tags = TagsList,

            });
        }


        public async Task<ActionResult> Details()
        {

            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            // return RedirectToAction("details", "user", new { id = id });
            return View(user);
        }
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRoles = await UserManager.GetRolesAsync(user.Id);
        //    var editableRoles = RoleManager.Roles.Except()
            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,    
                UserName = user.UserName,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,Address,City,State,Country,PostalCode,PhoneNumber,UserName")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Address = editUser.Address;
                user.City = editUser.City;
                user.PhoneNumber = editUser.PhoneNumber;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;
                user.UserName = editUser.UserName;
                user.Country = editUser.Country;
                string emailAddress = editUser.Email;
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }


                Customers cust = new Customers();
                cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
                /* Temporary code to add the already registered users to the customer table
                 *  , after that every user will be added automatically when registering */
                if (cust == null)
                {
                    cust = new Customers { UserName = user.UserName, Email = user.Email, PhoneNum = user.PhoneNumber };
                    db.Customers.Add(cust);
                    cust.Locations = new List<Locations>();
                }

                cust.FullName = editUser.UserName;
                cust.PhoneNum = editUser.PhoneNumber;
                cust.UserName = editUser.UserName;
                db.SaveChanges();
                adb.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }


        public async Task<ActionResult> Tags()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            List<Tags> tags = new List<Tags>();
            tags = cust.Tags.ToList();
           
            List<string> tagsNames = new List<string>();
            foreach (var item in tags)
            {
                tagsNames.Add(item.Name);
            }
            ViewBag.tags = tagsNames;
            return View();
        }

        [HttpGet]
        public async  Task<ActionResult> AddTags()
        {
            List<Tags> tags = new List<Tags>();
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            //Customers  cust = Session["Customer"] as Customers;
            tags = cust.Tags.ToList();
           List<string> tagsNames = new List<string>();
            foreach (var item in tags)
            {
                tagsNames.Add(item.Name);
            }

            var TagsList = db.Tags.ToList().Select(x => new SelectListItem()
            {
                Selected = tagsNames.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });

            Session["Customer "] = cust;
            ViewBag.tagsList = TagsList;
         //   return View();
            return View(new editCustomer
            {
                FullName = cust.FullName,
                Email = cust.Email,
                PhoneNum = cust.PhoneNum,
                Id = cust.Id,
                UserName = cust.UserName,
                tags = TagsList,

            });
        }
        [HttpPost]
        public  async Task<ActionResult> AddTags(  editCustomer newcust, params string[] selectedTags)
        {
            Collection<Tags> tags = new Collection<Tags>();
            selectedTags = selectedTags ?? new string[] { };
            //Customers cust = Session["Customer"] as Customers;
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            foreach (var item in selectedTags)
            {
                Tags tag = db.Tags.Where(x => x.Name == item).FirstOrDefault();
                tags.Add(tag);
            }
            if (cust.Tags.Count() > 0)
            {
                foreach(var tag in cust.Tags.ToList())
                {
                    cust.Tags.Remove(tag);
                }
            }
            foreach (var item in tags) {cust.Tags.Add(item);}
            db.SaveChanges();
            Session["Customer"] = cust;
            List<string> TagNames = new List<string>();
            //return View("TagsAdded");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> AddPreferences()
        {
          //  Collection<Tags> tags = new Collection<Tags>();
       //     selectedTags = selectedTags ?? new string[] { };
            //Customers cust = Session["Customer"] as Customers;
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            List<string> countries = new List<string>();
            countries.Add("Select");
            List<string> cities = new List<string>();
            cities.Add("Select");
            List<string> tags = new List<string>();
            tags.Add("Select");
            foreach (var item in db.Locations.ToList())
            {
                string country = item.Country;
                countries.Add(country);
                string city = item.City;
                cities.Add(city);
            }
            foreach (var item in db.Tags.ToList())
            {
                string tt = item.Name;
                tags.Add(tt);
            }
            countries = countries.Distinct().ToList();
            cities = cities.Distinct().ToList();
            SelectList mycountries = new SelectList(countries);
            SelectList mycities = new SelectList(cities);
            SelectList mytags = new SelectList(tags);
            ViewBag.listCountry = mycountries;
            Session["mycountries"] = mycountries;
            ViewBag.listCity = mycities;
            Session["mycities"] = mycities;
            ViewBag.taggs = mytags;
            Session["mytags"] = mytags;
            return View();
           
        }

        [HttpPost]
        public async Task<ActionResult> AddPreferences(FormCollection form)
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            cust.Preferences = new List<Preferences>();

            string country = form["listCountry"].ToString();
            string city = form["listCity"].ToString();
            string tag = form["taggs"].ToString();
            Tags tt = db.Tags.Where(x => x.Name == tag).FirstOrDefault();
            Preferences pref = new Preferences
            {
                Tag =tag, 
                Country = country,
                city = city
            };
            cust.Preferences.Add(pref);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public async  Task<ActionResult> Preferences()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            Customers cust = new Customers();
            cust = db.Customers.Where(x => x.Email == emailAddress).FirstOrDefault();
            List<Preferences> list = new List<Preferences>();
            foreach ( var item in cust.Preferences.ToList())
            {
                list.Add(item);
            }
            ViewBag.prefs = list;
            return View();
        }       
    }
}