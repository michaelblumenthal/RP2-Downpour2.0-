using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialUproarWebVote.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a Windows 10 IoT Core and ASP.NET and Azure Web Apps Demo page";

            return View();
        }

        //[Route("HomeController/Vote")]
        public ActionResult Vote()
        {
            
            ViewBag.Message = "Thank you for voting.";

            return View();
        }
    }
}