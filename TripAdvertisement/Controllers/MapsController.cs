using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TripAdvertisement.DAL;
using TripAdvertisement.Models;

namespace TripAdvertisement.Controllers
{
    public class MapsController : Controller
    {
        TripAdvertisementContext db = new TripAdvertisementContext();

        List<Coordinates> LocationsList = new List<Coordinates>();
        string userLatitude;
        string userLongtitude;
        
        // GET: Maps
        public ActionResult Index()
        {
          
            
            IEnumerable<SelectListItem> ListTypes = new List<SelectListItem>()
            {
             new SelectListItem {Text="Select",Value="",Selected=true },
                new SelectListItem {Text="Bar",Value="Battery" },
                new SelectListItem {Text="Restaurant",Value="Mouse"},
                new SelectListItem {Text="Cafe",Value="Keyboard"},
                new SelectListItem {Text="ATM",Value="Tower" },
                new SelectListItem {Text="Bakery",Value="Power Cord" },
                new SelectListItem {Text="Bank",Value="Power Supply" },
                new SelectListItem {Text="university",Value="university" },
                new SelectListItem {Text="police",Value="police" },
                new SelectListItem {Text="mosque",Value="mosque" },
                new SelectListItem {Text="airport",Value="airport" },
                new SelectListItem {Text="hospital",Value="hospital" },
                new SelectListItem {Text="parking",Value="parking" },
                new SelectListItem {Text="night_club",Value="night_club" },
                 new SelectListItem {Text="Other",Value="Other" },
            };
            IEnumerable<SelectListItem> ListRatings = new List<SelectListItem>()
            {
                new SelectListItem {Text="Select",Value="",Selected=true },
                new SelectListItem {Text="5",Value="5" },
                new SelectListItem {Text="4",Value="4" },
                new SelectListItem {Text="3",Value="3" },
                new SelectListItem {Text="2",Value="2" },
                new SelectListItem {Text="1",Value="1" },
            };
            IEnumerable<SelectListItem> ListRadius = new List<SelectListItem>()
            {
                new SelectListItem {Text="Select",Value="",Selected=true },
                new SelectListItem {Text="500",Value="500" },
                new SelectListItem {Text="400",Value="400" },
                new SelectListItem {Text="300",Value="300" },
            };
            ViewBag.ListItem1 = ListTypes;
            ViewBag.ListItem2 = ListRatings;
            ViewBag.ListItem3 = ListRadius;

            UserLocation coordinates = Session["userLocation"] as UserLocation;
            string nearbyPlaceUrl = "https://maps.googleapis.com/maps/api/place/radarsearch/json?location=" + coordinates.Altitude.ToString(new CultureInfo("en-US")) + "," + coordinates.Longtitude.ToString(new CultureInfo("en-US")) + "&radius=5000&type=cafe&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
            WebClient clientt = new WebClient();
            string NearbyPlacesjsonstring = clientt.DownloadString(nearbyPlaceUrl);
            List<string> NearbyPlacesList = new List<string>();
            dynamic NearbyPlacesdynObj = JsonConvert.DeserializeObject(NearbyPlacesjsonstring);
            int i = 0;
            foreach (var dd in NearbyPlacesdynObj.results)
            {
                i++;
                string Placelat = dd.geometry.location.lat;
                string Placelng = dd.geometry.location.lng;
                string PlaceId = dd.place_id;
                /* new api key ****** AIzaSyA0VzEDZ0bYCK9HnjPrfs0gGIswuVbtPko ***************/
                /* old api key ****** AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA ***************/



                string SearchPlaceDetailsUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + PlaceId + "&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
                WebClient newClient = new WebClient();
                newClient.Encoding = Encoding.GetEncoding("UTF-8");
                string PlaceDetails = newClient.DownloadString(SearchPlaceDetailsUrl);
                dynamic DetailsDynObj = JsonConvert.DeserializeObject(PlaceDetails);
                string PlaceName = DetailsDynObj.result.name;
                string PlaceAddress = DetailsDynObj.result.formatted_address;
                string PlacePhoneNum = DetailsDynObj.result.formatted_phone_number;
                string PlaceInternationalPhoneNum = DetailsDynObj.result.international_phone_number;
                string placeRating = DetailsDynObj.result.rating;
                string PlaceIconUrl = DetailsDynObj.result.icon;
                dynamic PlaceOpenNow = DetailsDynObj.result.opening_hours;
                bool OpenNow;
                if (PlaceOpenNow != null)
                {
                    OpenNow = PlaceOpenNow.open_now;
                }
                dynamic PlaceTypes = DetailsDynObj.result.types;
                List<string> types = new List<string>();
                foreach (var str in PlaceTypes)
                {
                    types.Add(str.ToString());
                }
                dynamic PlacePhotos = DetailsDynObj.result.photos;
                List<string> PlacePhotosLinks = new List<string>();
                if (PlacePhotos != null)
                {
                    foreach (var pd in PlacePhotos)
                    {
                        string photoLink = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + pd.photo_reference + "&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";

                        PlacePhotosLinks.Add(photoLink);
                    }
                }
                Coordinates PlaceCoordinates = new Coordinates
                {
                    Name = PlaceName,
                    Altitude = Placelat,
                    Address = PlaceAddress,
                    PhoneNum = PlacePhoneNum,
                    InternationalPhoneNum = PlaceInternationalPhoneNum,
                    IconUrl = PlaceIconUrl,
                    Id = PlaceId,
                    Longtitude = Placelng,
                    //OpenNow = PlaceOpenNow.open_now,
                    PhotoUrls = PlacePhotosLinks,
                    Rating = placeRating,
                    Types = types

                };
                LocationsList.Add(PlaceCoordinates);
                if (i > 10) break;
                //Vtechclub key : AIzaSyDa0r9CE6WhDcecsXEkvp83IhnwdGhx_uY
            }
            Session["LocationList"] = LocationsList;

            return View(db.Locations.ToList());
           
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {

            string str1 = form["ListItem1"].ToString();
            string str2 = form["ListItem2"].ToString();
            UserLocation userLoc = Session["userLocation"] as UserLocation;
            LocationsList = Session["LocationList"] as List<Coordinates>;
            return Json(LocationsList, JsonRequestBehavior.AllowGet);
          //  return View();
        }


        #region [Map]    
        [HttpPost]
        public JsonResult GetMap()
        {
            var data1 = Map();
            return Json(data1, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<Locations> Map()
        {

            return (from p in db.Locations
                    select new
                    {
                        Name = p.Name,
                        Latitude = p.Latitude,
                        Longitude = p.Logtitude,
                        Area = p.Area,
                        Description = p.Description,
                        Id = p.Id
                    }).ToList()
                .Select(res => new Locations
                {
                    Name = res.Name,
                    Latitude = res.Latitude,
                    Logtitude = res.Longitude,
                    Area=res.Area,
                    Description = res.Description,
                    Id = res.Id


                });

        }
        #endregion


        public ActionResult LocateUser(UserLocation coordinates )
        {
            
            
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + coordinates.Altitude.ToString(new CultureInfo("en-US")) + "," + coordinates.Longtitude.ToString(new CultureInfo("en-US")) + "&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
            Session["userLocation"] = coordinates;
            userLatitude = coordinates.Altitude.ToString();
            userLongtitude = coordinates.Longtitude.ToString();
            WebClient client = new WebClient();
            string jsonstring = client.DownloadString(url);
            List<string> locList = new List<string>();
            dynamic dynObj = JsonConvert.DeserializeObject(jsonstring);
            foreach( var data in dynObj.results)
            {
                string loc = data.formatted_address;
                locList.Add(loc);
            }
            /*Test search link , brings up to 200 results but not in the detailed way , it provides just Id , placeId , placeReference and alt and longtitude 
             * we could use place Id from there to use it in searchplaces link , to get that place details ( name , exact address , type .....)*/
 
           string nearbyPlaceUrl = "https://maps.googleapis.com/maps/api/place/radarsearch/json?location=" + coordinates.Altitude.ToString(new CultureInfo("en-US")) + "," + coordinates.Longtitude.ToString(new CultureInfo("en-US")) + "&radius=5000&type=cafe&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
           /*  Nearby places search link will provide detailed results but not more than 20 results */
            string NearestLocationsUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + coordinates.Altitude.ToString(new CultureInfo("en-US")) + "," + coordinates.Longtitude.ToString(new CultureInfo("en-US")) + "&radius=500&type=restaurant&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
                WebClient clientt = new WebClient();
            string NearbyPlacesjsonstring = clientt.DownloadString(nearbyPlaceUrl);
            List<string> NearbyPlacesList = new List<string>();
            dynamic NearbyPlacesdynObj = JsonConvert.DeserializeObject(NearbyPlacesjsonstring);
            int i = 0;
            foreach(var dd in NearbyPlacesdynObj.results)
            {
                i++;
                string Placelat = dd.geometry.location.lat;
                string Placelng =dd.geometry.location.lng;
                string PlaceId = dd.place_id;
                /* new api key ****** AIzaSyA0VzEDZ0bYCK9HnjPrfs0gGIswuVbtPko ***************/
                /* old api key ****** AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA ***************/
                


                string SearchPlaceDetailsUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid="+PlaceId+ "&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";
                WebClient newClient = new WebClient();
                newClient.Encoding = Encoding.GetEncoding("UTF-8");
                string PlaceDetails = newClient.DownloadString(SearchPlaceDetailsUrl);
                dynamic DetailsDynObj = JsonConvert.DeserializeObject(PlaceDetails);
                string PlaceName = DetailsDynObj.result.name;
                string PlaceAddress = DetailsDynObj.result.formatted_address;
                string PlacePhoneNum = DetailsDynObj.result.formatted_phone_number;
                string PlaceInternationalPhoneNum = DetailsDynObj.result.international_phone_number;
                string placeRating = DetailsDynObj.result.rating;
                string PlaceIconUrl = DetailsDynObj.result.icon;
                dynamic PlaceOpenNow = DetailsDynObj.result.opening_hours;
                bool OpenNow;
                if (PlaceOpenNow != null)
                {
                    OpenNow = PlaceOpenNow.open_now;
                }
                dynamic PlaceTypes = DetailsDynObj.result.types;
                List<string> types = new List<string>();
                foreach(var str in PlaceTypes)
                {
                    types.Add(str.ToString());
                }
                dynamic PlacePhotos = DetailsDynObj.result.photos;
                List<string> PlacePhotosLinks = new List<string>();
                if (PlacePhotos != null)
                {
                    foreach (var pd in PlacePhotos)
                    {
                        string photoLink = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + pd.photo_reference + "&key=AIzaSyCnNPFUjPwxKkpu36GSZsiWG2H1UszyVyk";

                        PlacePhotosLinks.Add(photoLink);
                    }
                }
                Coordinates PlaceCoordinates = new Coordinates
                {
                    Name = PlaceName,
                    Altitude = Placelat,
                    Address = PlaceAddress,
                    PhoneNum = PlacePhoneNum,
                    InternationalPhoneNum = PlaceInternationalPhoneNum,
                    IconUrl = PlaceIconUrl,
                    Id = PlaceId,
                    Longtitude = Placelng,
                    //OpenNow = PlaceOpenNow.open_now,
                    PhotoUrls = PlacePhotosLinks,
                    Rating = placeRating,
                    Types = types

                };
                LocationsList.Add(PlaceCoordinates);
                if (i > 10) break;
                //Vtechclub key : AIzaSyDa0r9CE6WhDcecsXEkvp83IhnwdGhx_uY
            }

            //string urll = "https://maps.googleapis.com/maps/api/geocode/json?latlng="+ lat + "," + lng + "&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";
            //WebClient client1 = new WebClient();
            //string jsonstringg = client1.DownloadString(urll);
            //List<string> locListt = new List<string>();
            //dynamic dynObjj = JsonConvert.DeserializeObject(jsonstringg);
            //foreach (var data in dynObjj.results)
            //{
            //    string locc = data.formatted_address;
            //    locListt.Add(locc);
            //}

            Session["LocationList"] = LocationsList;
            
                return null;
        }

        public ActionResult Search()
        {

            /*Test search link , brings up to 200 results but not in the detailed way , it provides just Id , placeId , placeReference and alt and longtitude 
             * we could use place Id from there to use it in searchplaces link , to get that place details ( name , exact address , type .....)*/
            UserLocation userLoc = Session["userLocation"] as UserLocation;
            LocationsList = Session["LocationList"] as List<Coordinates>;
            return Json(LocationsList, JsonRequestBehavior.AllowGet);












            //string nearbyPlaceUrl = "https://maps.googleapis.com/maps/api/place/radarsearch/json?location=" + userLatitude+ "," + userLongtitude + "&radius=5000&type=cafe&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";
            ///*  Nearby places search link will provide detailed results but not more than 20 results */
            //string NearestLocationsUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + userLatitude + "," + userLongtitude + "&radius=500&type=restaurant&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";
            //WebClient clientt = new WebClient();
            //string NearbyPlacesjsonstring = clientt.DownloadString(nearbyPlaceUrl);
            //List<string> NearbyPlacesList = new List<string>();
            //dynamic NearbyPlacesdynObj = JsonConvert.DeserializeObject(NearbyPlacesjsonstring);
            //foreach (var dd in NearbyPlacesdynObj.results)
            //{

            //    string Placelat = dd.geometry.location.lat;
            //    string Placelng = dd.geometry.location.lng;
            //    string PlaceId = dd.place_id;
            //    string SearchPlaceDetailsUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + PlaceId + "&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";
            //    WebClient newClient = new WebClient();
            //    string PlaceDetails = newClient.DownloadString(SearchPlaceDetailsUrl);
            //    dynamic DetailsDynObj = JsonConvert.DeserializeObject(PlaceDetails);
            //    string PlaceName = DetailsDynObj.result.name;
            //    string PlaceAddress = DetailsDynObj.result.formatted_address;
            //    string PlacePhoneNum = DetailsDynObj.result.formatted_phone_number;
            //    string PlaceInternationalPhoneNum = DetailsDynObj.result.international_phone_number;
            //    string placeRating = DetailsDynObj.result.rating;
            //    string PlaceIconUrl = DetailsDynObj.result.icon;
            //    dynamic PlaceOpenNow = DetailsDynObj.result.opening_hours;
            //    if (PlaceOpenNow != null)
            //    {
            //        bool OpenNow = PlaceOpenNow.open_now;
            //    }
            //    dynamic PlaceTypes = DetailsDynObj.result.types;
            //    List<string> types = new List<string>();
            //    foreach (var str in PlaceTypes)
            //    {
            //        types.Add(str.ToString());
            //    }
            //    dynamic PlacePhotos = DetailsDynObj.result.photos;
            //    List<string> PlacePhotosLinks = new List<string>();
            //    if (PlacePhotos != null)
            //    {
            //        foreach (var pd in PlacePhotos)
            //        {
            //            string photoLink = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + pd.photo_reference + "&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";

            //            PlacePhotosLinks.Add(photoLink);
            //        }
            //    }
            //    Coordinates PlaceCoordinates = new Coordinates
            //    {
            //        Name = PlaceName,
            //        Altitude = Placelat,
            //        Address = PlaceAddress,
            //        PhoneNum = PlacePhoneNum,
            //        InternationalPhoneNum = PlaceInternationalPhoneNum,
            //        IconUrl = PlaceIconUrl,
            //        Id = PlaceId,
            //        Longtitude = Placelng,
            //        OpenNow = PlaceOpenNow,
            //        PhotoUrls = PlacePhotos,
            //        Rating = placeRating,
            //        Types = PlaceTypes

            //    };
            //    LocationsList.Add(PlaceCoordinates);
            //}

            //string urll = "https://maps.googleapis.com/maps/api/geocode/json?latlng="+ lat + "," + lng + "&key=AIzaSyAQyR6rrSbBejLcieCfEAosWHbfINc8HqA";
            //WebClient client1 = new WebClient();
            //string jsonstringg = client1.DownloadString(urll);
            //List<string> locListt = new List<string>();
            //dynamic dynObjj = JsonConvert.DeserializeObject(jsonstringg);
            //foreach (var data in dynObjj.results)
            //{
            //    string locc = data.formatted_address;
            //    locListt.Add(locc);
            //}

        }
    }
}