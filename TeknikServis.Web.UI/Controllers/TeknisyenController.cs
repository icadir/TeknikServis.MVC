using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.Controllers
{
    public class TeknisyenController : Controller
    {
        // GET: Teknisyen
        public ActionResult Index()
        {
            //Sistemdeki teknisyenId sini verir.
           var teknisyenId= HttpContext.User.Identity.GetUserId();
           var data = new ArizaKayitRepo()
               .GetAll(x => x.TeknisyenId == teknisyenId).Select(x=>Mapper.Map<ArizaViewModel>(x))
               .ToList();

            return View(data);
        }

        public ActionResult ArizaRapor(int id)
        {
            try
            {

            }
            catch (Exception ex)
            {
               
            }

            return View();
        }
    }
}