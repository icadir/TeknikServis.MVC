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
        [ValidateAntiForgeryToken]
        public ActionResult Index()
        {
            //TODO sadece müşteri giricek ayarlarsın.
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArizaKayitEkle(ArizaViewModel model)
        {

            return View();
        }
    }
}