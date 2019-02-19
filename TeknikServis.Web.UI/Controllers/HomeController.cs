using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeknikServis.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Harita()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Team()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
    }
}