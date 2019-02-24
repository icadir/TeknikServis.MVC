using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeknikServis.BLL.Helpers;
using TeknikServis.BLL.Repository;
using TeknikServis.BLL.Services.Senders;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

namespace TeknikServis.Web.UI.Controllers
{
    [Authorize(Roles = "Admin,Operator,Teknisyen")]
    public class TeknisyenController : BaseController
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
                data.ArızaPath = ariza.Fotograflar.Select(y => y.Yol).ToList();
                //ilgili log kayıtları getiriyor. o arizaya ait.
                var Logs = new ArizaLogRepo().GetAll()
                    .Where(u => u.ArızaId == id)
                    .OrderByDescending(u => u.CreatedDate)
                    .ToList();
                data.ArizaLogs.Clear();

                //data.ArizaLogViewModels.Clear();
                //gelen logkayitlarini mapper ile view model e çevirip. arizaviewmoedeki alana ekliyoruz.
                //BURADA VİEWMODEL YAPAMADIK NEDEN BAK SOR ...
                foreach (ArizaLOG log in Logs)
                {

                    data.ArizaLogs.Add(log);
                }
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
                var TeknisyenId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var teknisyen = await NewUserManager().FindByIdAsync(TeknisyenId);
                var musteri = NewUserManager().FindById(model.MusteriId);
                var Operator = NewUserManager().FindById(model.OperatorId);
                //musteri ıd vs gelmiyor onları getir . userı bul mail e gönder

                if (model.TeknisyenArizaDurum == TeknisyenArizaDurum.Çözüldü)
                {
                    ariza.ArizaCozulduguTarih = DateTime.Now;
                    var user = NewUserManager().FindById(model.MusteriId);
                    ariza.TeknisyenDurumu = TeknisyenDurumu.Bosta;
                    ariza.AnketCode = StringHelpers.GetCode();
                    new ArizaKayitRepo().Update(ariza);

               
                    string SiteUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
                                     (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    var emailService = new EmailService();
                    var body = $"Merhaba {musteri.Name} {musteri.Surname} <b></b><br>FİTech için geri döüşleriniz çok önemli. 5 Dakikanızı ayırarak anketimizi doldurabilirsiniz. Aşagıdaki Linke tıklayarak anket sayfasına gidebilirsiniz.<br> <a href='{SiteUrl}/Anket/Index?code={ariza.AnketCode}' >Anket'e Gitmek için Tıklayınız. </a> ";
                   
                    await emailService.SendAsync(new IdentityMessage()
                    {
                        
                        Body = body,
                        Subject = "Anket"
                    }, model.Email);

                    //TODO Acaba buradada kontrol eedildimi edilmedimi kontrol etmeye gerek var mı ?
                    var body2 = $"Merhaba {Operator.Name} {Operator.Surname} <b></b><br>{teknisyen.Name} {teknisyen.Surname} isimli calısanımıza {model.TeknisyenAtandigiTarih}'de atamıs oldugunuz Arizanın çözüldügünü iletti. Kontrol etmek için aşagıdaki link'e tıklayınız.<br> <a href='{SiteUrl}/TeknisyenArizaRapor/{model.ArizaId}' >Kontrol Etmek için Tıklayınız. </a> ";
                    await emailService.SendAsync(new IdentityMessage()
                    {
                        Body = body,
                        Subject = "Kontrol"
                    }, Operator.Email);
                }

                var TeknisyenLog = new ArizaLOG
                {
                    ArızaId = model.ArizaId,
                    YapanınRolu = IdentityRoles.Teknisyen,
                    CreatedDate = DateTime.Now,
                    Aciklama = $"Arıza Kontrol Edildi. Kontrol sonucu {model.TeknisyenArizaAciklama} notu bırakıldı .Son Durum {model.TeknisyenArizaDurum}",

                };
                new ArizaLogRepo().Insert(TeknisyenLog);
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