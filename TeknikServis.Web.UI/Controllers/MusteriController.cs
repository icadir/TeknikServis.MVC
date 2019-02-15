using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.Controllers
{
    public class MusteriController : Controller
    {
        // GET: Musteri
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ArizaKayitEkle(ArizaViewModel model)
        {
            return View();
        }
    }
}