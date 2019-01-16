using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TripAdvertisement.Controllers
{
    public class RedirectController : Controller
    {
        List<string> roles;
        public ActionResult Index()
        {
            roles = new List<string>();

            int s = 0;
            if (User.IsInRole("Master"))
            {
                roles.Add("Master");
                s++;
            }

            if (User.IsInRole("User"))
            {
                s++;
                roles.Add("User");
            }
            //if (User.IsInRole("Technician"))
            //{
            //    roles.Add("Technician");
            //    s++;
            //}
            Session["roles"] = roles;
            if (s > 1)
                return RedirectToAction("ContinueAs");
            else
            {


                if (User.IsInRole("Master"))
                {
                    return RedirectToAction("Index", "Home");
                }

                if (User.IsInRole("User"))
                    return RedirectToAction("Index", "Reception");

                //if (User.IsInRole("Technician"))
                //    return RedirectToAction("Index", "TechOperations");

                return RedirectToAction("Index","Home");

            }
        }


        public ActionResult ContinueAs()
        {
            List<string> roles = Session["roles"] as List<string>;
            ViewBag.roles = new SelectList(roles);
            return View();
        }


        [ValidateInput(true)]
        [HttpPost]
        public ActionResult ContinueAs(FormCollection form)
        {
            string str = form["roles"].ToString();
            if (str == "Master")
                return RedirectToAction("Index", "Home");
            if (str == "User")
                return RedirectToAction("Index", "Reception");
            //if (str == "Technician")
            //    return RedirectToAction("Index", "TechOperations");
            return View();
        }
    }
}