using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TripAdvertisement.DAL;
using TripAdvertisement.Models;

namespace TripAdvertisement.Controllers
{
    public class SearchController : Controller
    {
        private TripAdvertisementContext db = new TripAdvertisementContext();
        // GET: Search
        [HttpGet]
        public ActionResult Index()
        {
            List<string> countries = new List<string>();
            countries.Add("Select");
            List<string> cities = new List<string>();
            cities.Add("Select");
            List<string> tags = new List<string>();
            tags.Add("Select");
            foreach(var item in db.Locations.ToList())
            {
                string country = item.Country;
                countries.Add(country);
                string city = item.City;
                cities.Add(city);
            }
            foreach(var item in db.Tags.ToList())
            {
                string tt = item.Name;
                tags.Add(tt);
            }
           countries= countries.Distinct().ToList();
           cities= cities.Distinct().ToList();
            SelectList mycountries = new SelectList(countries);
            SelectList mycities = new SelectList(cities);
            SelectList mytags = new SelectList(tags);
            ViewBag.listCountry = mycountries;
            Session["mycountries"] = mycountries;
            ViewBag.listCity = mycities;
            Session["mycities"] = mycities;
            ViewBag.tags = mytags;
            Session["mytags"] = mytags;
            var list1 = db.Locations.ToList();
            return View(list1);
        }


        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string country = form["listCountry"].ToString();
            string city = form["listCity"].ToString();
            string tag = form["tags"].ToString();
            List<Locations> list1 = new List<Locations>();
            if ((tag == "Select") && (city == "Select") && (country == "Select"))
                ViewBag.error = "Hi there , our app isn't smart enought to search the void ! would you please choose parameters for ur research";
            list1 = db.Locations.ToList();
            if (tag != "Select")
            {
                Tags tt = db.Tags.Where(x => x.Name == tag).FirstOrDefault();
                if ((city != "Select") && (country != "Select"))
                    list1 = tt.Locations.Where(x => (x.City == city && x.Country == country)).ToList();
                else
                {
                    if (city != "Select") list1 = db.Locations.Where(x => (x.City == city)).ToList();
                    else list1 = db.Locations.Where(x => (x.Country == country)).ToList();
                }
            }
            else
            {
                if((city!="Select")&&(country!="Select"))
                list1 = db.Locations.Where(x => (x.City == city && x.Country == country)).ToList();
                else
                {
                    if(city!="Select")
                        list1 = db.Locations.Where(x => (x.City == city )).ToList();
                    else
                        list1 = db.Locations.Where(x => ( x.Country == country)).ToList();
                }
            }
           
            ViewBag.listCountry = Session["mycountries"] as SelectList;
            ViewBag.listCity = Session["mycities"] as SelectList;
            ViewBag.tags = Session["mytags"] as SelectList;
            return View(list1);
        }

    }
}