using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Helpers;
using TeknikServis.BLL.Repository;
using TeknikServis.BLL.Services.Senders;
using TeknikServis.Entity.Anket;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

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
                var teknisyenId = HttpContext.User.Identity.GetUserId();
                //arızalar tablosuna sistemdeki teknisyen ıd  alındı ve sorgu atıldı . mapper ile view model e çevrilip sayfaya gönderildi
                var data = new ArizaKayitRepo()
                    .GetAll(x => x.TeknisyenId == teknisyenId).Select(x => Mapper.Map<ArizaViewModel>(x))
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


                var ariza = await new ArizaKayitRepo().GetByIdAsync(model.ArizaId);
                if (model.TeknisyenArizaDurum == null)
                {
                    return RedirectToAction("TeknisyenArizaRapor", "Teknisyen", model);
                }
                ariza.TeknisyenArizaAciklama = model.TeknisyenArizaAciklama;

                ariza.TeknisyenArizaDurum = model.TeknisyenArizaDurum;
                ariza.ArizaSonKontrolTarihi = DateTime.Now;


                //musteri ıd vs gelmiyor onları getir . userı bul mail e gönder

                if (model.TeknisyenArizaDurum == TeknisyenArizaDurum.Çözüldü)
                {
                    ariza.ArizaCozulduguTarih = DateTime.Now;
                    var user = NewUserManager().FindById(model.MusteriId);
                    ariza.TeknisyenIstemi = false;
                    ariza.AnketCode = StringHelpers.GetCode();
                    new ArizaKayitRepo().Update(ariza);

                    string SiteUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
                                     (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    var x = new Anket();
                    var emailService = new EmailService();
                    var body = $"Merhaba <b></b><br>FİTech için geri döüşleriniz çok önemli. 5 Dakikanızı ayırarak anketimizi doldurabilirsiniz. Aşagıdaki Linke tıklayarak anket sayfasına gidebilirsiniz.<br> <a href='{SiteUrl}/Anket/Index?code={ariza.AnketCode}' >Anket'e Gitmek için Tıklayınız. </a> ";
                    await emailService.SendAsync(new IdentityMessage()
                    {
                        Body = body,
                        Subject = "Anket"
                    }, user.Email);

                }


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