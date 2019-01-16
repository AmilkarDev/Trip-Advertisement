using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using TripAdvertisement.DAL;
using TripAdvertisement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using PagedList;
using System.Web.Mvc;

namespace TripAdvertisement.Controllers
{
    public class LocationsController : Controller
    {
        private TripAdvertisementContext db = new TripAdvertisementContext();
        public LocationsController() { }

        public LocationsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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

        // GET: Locations
        public ActionResult Index(int page = 1, int pageSize = 3)
        {
           var list = db.Locations.ToList();
            PagedList<Locations> model = new PagedList<Locations>(list, page, pageSize);
            return View(model);
        }

        // GET: Locations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locations locations = db.Locations.Find(id);
            
            if (locations == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<string> tagnames = new List<string>();
                var tags = locations.Tags.ToList();
                foreach (var item in tags)
                    tagnames.Add(item.Name);

            ViewBag.TagNames = tagnames;
            return View(locations);
            }
        }

        // GET: Locations/Create
        public ActionResult Create()
        {
            var tags = db.Tags.ToList();
            Locations loc = new Locations
            {
                Name="Miami",
                Address="collagiate way",
                Area="south Beach",
                City="Miami",
                AverageTime="hour and half",
                Country="United States",
                Description="Awesome",
                Latitude="1200",
                Logtitude="1500",
                Tags = tags,
            };
            return View(loc);
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,ImageLink,Address,Area,Latitude,Logtitude,Description,Country,City,AverageTime")] Locations locations, HttpPostedFileBase upload, params string[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                selectedTags = selectedTags ?? new string[] { };

                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new Images
                    {
                        Name= System.IO.Path.GetFileName(upload.FileName),
                        Link = "~/Images/" + upload.FileName,
                        Date=DateTime.Now.Date.ToShortDateString(),
                        Description= locations.Description,
                        LocationId=locations.Id,
                        Locations=locations
                    };
                    photo.Link = "~/Images/" + photo.Name;
                    upload.SaveAs(Server.MapPath(photo.Link));
                    db.Images.Add(photo);
                    locations.Images = new Collection<Images>();
                    locations.Tags = new Collection<Tags>();
                    locations.Images.Add(photo);
                    locations.ImageLink = photo.Link;
                    Tags tag = new Tags();
                    foreach (var item in selectedTags)
                    {
                        tag = db.Tags.Where(x => x.Name == item).ToList().FirstOrDefault();
                        locations.Tags.Add(tag);
                    }
                }
               
                db.Locations.Add(locations);
               

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
                if(cust==null)
                {
                    cust = new Customers {UserName= user.UserName, Email=user.Email, PhoneNum=user.PhoneNumber };
                    db.Customers.Add(cust);
                    cust.Locations = new List<Locations>();
                }
              
                cust.Locations.Add(locations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locations);
        }

        [HttpGet]
        public ActionResult AddPhotos(int Id)
        {
            Locations loc = db.Locations.Where(x => x.Id == Id).Include(c=>c.Images).FirstOrDefault() ;
            Session["Location"] = loc;
            return View();
        }

        [HttpPost]

        public ActionResult AddPhotos(int Id, HttpPostedFileBase upload)
        {
            Locations loc = db.Locations.Where(x => x.Id == Id).FirstOrDefault();
           //Locations Loca = Session["Location"] as Locations;
            if (upload != null && upload.ContentLength > 0)
            {
                var photo = new Images
                {
                    Name = System.IO.Path.GetFileName(upload.FileName),
                    Link = "~/Images/" + upload.FileName,
                    Date = DateTime.Now.Date.ToShortDateString(),
                    Description = loc.Description,
                    LocationId = loc.Id,
                    Locations = loc
                };
             //   photo.Link = "~/Images/" + photo.Name;
                upload.SaveAs(Server.MapPath(photo.Link));
                db.Images.Add(photo);
                loc.Images = new Collection<Images>();
                loc.Images.Add(photo);
              //  locations.ImageLink = photo.Link;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult ShowPhotos( )
        {
            Locations loc = Session["Location"] as Locations;
            var photos = loc.Images.ToList();
            return View(photos);
        }

        // GET: Locations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locations locations = db.Locations.Find(id);
            var tags = locations.Tags.ToList();
            List<string> tagnames = new List<string>();
            foreach (var item in tags)
                tagnames.Add(item.Name);
            if (locations == null)
            {
                return HttpNotFound();
            }
            return View(new editLocation{
                Name=locations.Name,
                Description=locations.Description,
                Address=locations.Address,
                City=locations.City,
                Country=locations.Country,
                AverageTime=locations.AverageTime,
                Area=locations.Area,
                ImageLink=locations.ImageLink,
                Id=locations.Id,
                Latitude=locations.Latitude,
                Logtitude= locations.Logtitude,
               tags=db.Tags.ToList().Select(x=> new SelectListItem(){
                   Selected = tagnames.Contains(x.Name),
                   Value=x.Name,
                   Text=x.Name
               })
            });
            
            
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ImageLink,Address,Area,Latitude,Logtitude,Description,Country,City,AverageTime")] editLocation locations, params string[] SelectedTags)
        {
            if (ModelState.IsValid)
            {

                Locations location = db.Locations.Where(x => x.Id == locations.Id).FirstOrDefault();
                if (location == null) { return HttpNotFound(); }

                location.Id = locations.Id;
                location.ImageLink = locations.ImageLink;
                location.Name = locations.Name;
                location.Address = locations.Address;
                location.Area = locations.Area;
                location.Latitude = locations.Latitude;
                location.Logtitude = locations.Logtitude;
                location.Description = locations.Description;
                location.Country = locations.Country;
                location.City = locations.City;
                location.AverageTime = locations.AverageTime;

              
                Collection<Tags> tags = new Collection<Tags>();
                SelectedTags = SelectedTags ?? new string[] { };
                
                foreach (var item in SelectedTags)
                {
                    Tags tag = db.Tags.Where(x => x.Name == item).FirstOrDefault();
                    tags.Add(tag);
                }

                if (location.Tags.Count > 0)
                {
                    foreach (var item in location.Tags.ToList())
                        location.Tags.Remove(item);
                }

                foreach (var item in tags)
                {
                    location.Tags.Add(item);
                }
                //location.Tags = userTags;
               

                db.Entry(location).State = EntityState.Modified;
                //db.Entry(locations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locations);
        }

        // GET: Locations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locations locations = db.Locations.Find(id);
            if (locations == null)
            {
                return HttpNotFound();
            }
            return View(locations);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Locations locations = db.Locations.Find(id);
            db.Locations.Remove(locations);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Images/Delete/5
        public ActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Images images = db.Images.Find(id);
            if (images == null)
            {
                return HttpNotFound();
            }
            return View(images);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("DeleteImage")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImageConfirmed(int id)
        {
            Images images = db.Images.Find(id);
            db.Images.Remove(images);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
