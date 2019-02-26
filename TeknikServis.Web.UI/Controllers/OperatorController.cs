using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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
    [Authorize(Roles = "Admin,Operator")]
    public class OperatorController : BaseController
    {
        // GET: Operator
        public ActionResult Index()
        {

            var data = new ArizaKayitRepo()
                .GetAll(x => x.OperatorKabul == false)
                .Select(x => Mapper.Map<ArizaViewModel>(x))
                .ToList();

            return View(data);
        }

        [HttpGet]
        public async Task<ActionResult> ArizaDetay(int id)
        {
            try
            {

                var x = new ArizaKayitRepo().GetById(id);

                var data = Mapper.Map<ArizaViewModel>(x);
                data.ArızaPath= new FotografRepo().GetAll(z => z.ArizaId == id).Select(u=>u.Yol).ToList();

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Admin",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<ActionResult> ArizaKabul(int id)
        {
            //ikisiylede bulabilirsin o anki sistemde online olanı
            //var OpertatorId = HttpContext.User.Identity.GetUserId();

            var OpertatorId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                var ariza = await new ArizaKayitRepo().GetByIdAsync(id);
                var Operator = await NewUserManager().FindByIdAsync(OpertatorId);
                if (ariza == null)
                {
                    RedirectToAction("Index", "Operator");
                }
                else
                {
                    ariza.OperatorKabulTarih = DateTime.Now;
                    ariza.OperatorKabul = true;
                    ariza.OperatorId = OpertatorId;
                    ariza.ArizaDurumu = ArizaDurum.OperatorTakibeAldı;
                    new ArizaKayitRepo().Update(ariza);
                    var OperatorLog = new ArizaLOG
                    {
                        CreatedDate = DateTime.Now,
                        ArızaId = id,
                        Aciklama = $"Ariza'nız {Operator.Name} {Operator.Surname} isimli Operator Tarafından Onaylanmıştır.",
                        YapanınRolu = IdentityRoles.Teknisyen
                    };
                    new ArizaLogRepo().Insert(OperatorLog);
                    return RedirectToAction("Index", "Operator");
                    //TODO Müşteriye Mail gönderilir bilgilendirme belki
                }

                return RedirectToAction("Index", "Operator");

            }

            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Admin",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpGet]
        public ActionResult ArizaList()
        {
            //operator bulunuyor ve o operatorun aldıgı kayıtlar listelenip çekiliyor.
            var operatorId = HttpContext.User.Identity.GetUserId();
            try
            {
                var data = new ArizaKayitRepo()
                    .GetAll(x => x.OperatorId == operatorId)
                    .Select(x => Mapper.Map<ArizaViewModel>(x))
                    .OrderBy(x => x.ArizaOlusturmaTarihi)
                    .ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Admin",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO TEknisyen atamayı burada yap
        public async Task<ActionResult> TeknisyenAta(ArizaViewModel model)
        {

            try
            {

                var ariza = new ArizaKayitRepo().GetById(model.ArizaId);
                ariza.TeknisyenId = model.UserId;
                ariza.ArizaDurumu = ArizaDurum.TeknisyenAtandi;
                ariza.TeknisyenDurumu = TeknisyenDurumu.Calısıyor;
                ariza.TeknisyenAtandigiTarih = DateTime.Now;
                new ArizaKayitRepo().Update(ariza);

                var opId = HttpContext.User.Identity.GetUserId();
                var teknisyen = NewUserManager().FindById(ariza.TeknisyenId);
                var userOperator = NewUserManager().FindById(opId);
                var musteri = NewUserManager().FindById(ariza.MusteriId);

                var OperatorLog = new ArizaLOG
                {
                    CreatedDate = DateTime.Now,
                    ArızaId = model.ArizaId,
                    Aciklama = $"Ariza'nız {userOperator.Name} {userOperator.Surname} isimli Operator Tarafından {teknisyen.Name} {teknisyen.Surname} Teknisyene Bildirilmiştir.",
                    YapanınRolu = IdentityRoles.Teknisyen
                };
                new ArizaLogRepo().Insert(OperatorLog);

                string SiteUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
                                 (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                var emailService = new EmailService();

                // teknisyene Mail gönderiyor test ok.
                #region TEKNİSYENE MAİL GÖNDERME OK

                var body = $"Merhaba <b> {teknisyen.Name} {teknisyen.Surname}</b><br>  Size {userOperator.Name} {userOperator.Surname} isimli çalışanımız tarafından bir Arıza Gönderilmiştir.  Aşagıdaki Linke tıklayarak arızayı detaylı bir şekilde görebilirsiniz..<br>İyi Çalışmalar dileriz. FiTech <br> <a href='{SiteUrl}/Teknisyen/TeknisyenArizaRapor/{ariza.Id}' >Arıza Detayları için tıklayınız. </a> ";
                await emailService.SendAsync(new IdentityMessage()
                {
                    Body = body,
                    Subject = "Ariza İş Bildirimi"
                }, teknisyen.Email);
                #endregion

                #region MÜSTERİYE MAİL GÖNDERME
                var bodyMusteri = $"Merhaba <b> {musteri.Name} {musteri.Surname}</b><br>  Sizin Acmıs oldugunuz {ariza.Id}'nolu kayıta {teknisyen.Name} {teknisyen.Surname} isimli çalışanımız yönlendirilmiştir<br> Fitech İyi Günler Diler.</a> ";
                await emailService.SendAsync(new IdentityMessage()
                {
                    Body = bodyMusteri,
                    Subject = "Ariza Teknisyen Bilgisi"
                }, model.Email);


                #endregion

                TempData["Message"] = $"{ariza.Id} nolu arızaya {teknisyen.Name}  {teknisyen.Surname} atanmıştır.İyi çalışmalar.";

                return RedirectToAction("Index", "Operator");
            }

            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Admin",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }

        }

        public ActionResult OPArizaDetay(int id)
        {

            try
            {
                var ariza = new ArizaKayitRepo().GetById(id);
                var data = Mapper.Map<ArizaViewModel>(ariza);
                data.ArızaPath = new FotografRepo().GetAll(z => z.ArizaId == id).Select(u => u.Yol).ToList();
                ViewBag.TeknisyenK = BostaOlanTeknisyenler(data);
               
               

                return View(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }
    }
}