using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.Controllers
{
    public class TeknisyenController : Controller
    {
        // GET: Teknisyen
        public ActionResult Index()
        {
            try
            {
                //Sistemdeki teknisyenId sini verir.
                var teknisyenId= HttpContext.User.Identity.GetUserId();
                //arızalar tablosuna sistemdeki teknisyen ıd  alındı ve sorgu atıldı . mapper ile view model e çevrilip sayfaya gönderildi
                var data = new ArizaKayitRepo()
                    .GetAll(x => x.TeknisyenId == teknisyenId).Select(x=>Mapper.Map<ArizaViewModel>(x))
                    .ToList();

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Home",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }


        //Burada Kaldık Sayfa geliyor fakat enum olarak sadece arızalarla alakalı bir enum yapmak gerekiyor. Çalışan enumlarda atanmadı vs seçebiliyor.
        public ActionResult TeknisyenArizaRapor(int id)
        {
            try
            {
                var ariza = new ArizaKayitRepo().GetById(id);
                var data = Mapper.Map<ArizaViewModel>(ariza);
                return View(data);

            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Home",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");

            }
       
        }
        [HttpGet]
        public async Task<ActionResult> TeknisyenArızaBildiriOnayla(ArizaViewModel model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    RedirectToAction("TeknisyenArizaRapor", "Teknisyen", model);
                //}

               var ariza= await new ArizaKayitRepo().GetByIdAsync(model.ArizaId);
               if (model.TeknisyenArizaDurum == null)
               {
                   return RedirectToAction("TeknisyenArizaRapor", "Teknisyen", model);
               }
               ariza.TeknisyenArizaAciklama = model.TeknisyenArizaAciklama;
               ariza.TeknisyenArizaDurum = model.TeknisyenArizaDurum;
               ariza.ArizaSonKontrolTarihi=DateTime.Now;
               if (model.TeknisyenArizaDurum == TeknisyenArizaDurum.Çözüldü)
               {
                    ariza.ArizaCozulduguTarih=DateTime.Now;
               }

               new ArizaKayitRepo().Update(ariza);
               TempData["Message"] = $"{model.ArizaId} no lu Kayıt Raporu Alınmıştır. İyi çalışamlar";
              return RedirectToAction("Index", "Teknisyen");

            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Teknisyen",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
     
        }

    }
}