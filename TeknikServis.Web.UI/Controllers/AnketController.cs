using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Anket;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.ViewModels.ArizaViewModels;


namespace TeknikServis.Web.UI.Controllers
{
    [Authorize(Roles = "Musteri")]
    public class AnketController : BaseController
    {
        // GET: Anket
        public ActionResult Index(string code)
        {
            var ariza = new ArizaKayitRepo().Queryable().FirstOrDefault(x => x.AnketCode == code);
         
            if (ariza == null)
            {
                TempData["Message"] = $"Böyle Bir Arıza Bulunamamıştır ";
                RedirectToAction("Index", "Home");
            }

            if (ariza.AnketYapildimi == true)
            {
                TempData["Message"] = $"Bu anket daha önceden doldurulmustur. ";
                RedirectToAction("Index", "Home");
            }

            //sistemdeki giriş yapmıs kişin idsını buluyorum.
            var MusteriId = HttpContext.User.Identity.GetUserId();
            //bulduugm Id  anket kodundaki buldugum id ile aynımı degilse gönderiyorum.
            if (ariza.MusteriId != MusteriId)
            {
                TempData["Message"] = $"Bu anket sizin arızanıza tanımlanmamıştır. ";
                RedirectToAction("Index", "Home");
            }
            


            return View(ariza);
        }

        public ActionResult AnketKabul(ArızaKayıt model)
        {
            var ariza = new ArizaKayitRepo().GetById(model.Id);
            if (ariza != null)
            {
                ariza.FitechDavranisPuani = model.FitechDavranisPuani;
                ariza.FitechHakkindakiGorusler = model.FitechHakkindakiGorusler;
                ariza.TeknisyenBilgiPuani = model.TeknisyenBilgiPuani;
                ariza.TeknisyenDavranisPuani = model.TeknisyenDavranisPuani;
                ariza.HizmetPuanı = model.HizmetPuanı;
                ariza.AnketYapildimi = true;
                new ArizaKayitRepo().Update(ariza);
                TempData["Message"] = $"Katıldıgınız için Teşekkür ederiz.";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["Message"] = $"Bir hata olustu.İyi günler";
                return RedirectToAction("Index", "Home");
            }

        }
    }
}