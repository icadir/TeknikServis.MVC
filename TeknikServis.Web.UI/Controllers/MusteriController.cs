using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Helpers;
using TeknikServis.BLL.Repository;
using TeknikServis.BLL.Services.Senders;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

namespace TeknikServis.Web.UI.Controllers
{
    [Authorize(Roles = "Admin,Musteri")]
    public class MusteriController : BaseController
    {
        // GET: Musteri

        public ActionResult Index()
        {
            //TODO sadece müşteri giricek ayarlarsın.
            //Sayfaya tıklayan kişinin giriş yapıp yapamadıgını kontrol eder. 
            //if (HttpContext.GetOwinContext().Authentication.User.Identity.IsAuthenticated)
          
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ArizaKayitEkle(ArizaViewModel model)
        {
            //o anki sistemdeki kullanıcının idsini verir.
            //var asd = HttpContext.User.Identity.GetUserId();
            //
            //if (!ModelState.IsValid)
            //{
            //    //Gelen model valid degiilse bu sayfaya yönlendirilip hatalar gösterilicek.
            //    return RedirectToAction("Index", "Musteri", model);
            //}

            var userManager = NewUserManager().FindById(model.MusteriId);
            try
            {
                #region FaturaResimİşlemleri
                if (model.PostedFileFatura != null &&
                    model.PostedFileFatura.ContentLength > 0)
                {
                    var file = model.PostedFileFatura;
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extName = Path.GetExtension(file.FileName);
                    fileName = StringHelpers.UrlFormatConverter(fileName);
                    fileName += StringHelpers.GetCode();
                    var klasoryolu = Server.MapPath("~/Fatura/");
                    var dosyayolu = Server.MapPath("~/Fatura/") + fileName + extName;

                    if (!Directory.Exists(klasoryolu))
                        Directory.CreateDirectory(klasoryolu);
                    file.SaveAs(dosyayolu);

                    WebImage img = new WebImage(dosyayolu);
                    img.Resize(250, 250, false);
                    img.AddTextWatermark("FİTech");
                    img.Save(dosyayolu);
                    var oldPath = model.FaturaPath;
                    model.FaturaPath = "/Fatura/" + fileName + extName;

                    System.IO.File.Delete(Server.MapPath(oldPath));
                }


                #endregion

                var data = new ArızaKayıt
                {
                    MusteriId = model.MusteriId,
                    Adres = model.Adres,
                    ArizaDurumu = model.ArizaDurumu,
                    ArizaOlusturmaTarihi = DateTime.Now,
                    ArızaAcıklaması = model.ArızaAcıklaması,
                    BeyazEsya = model.BeyazEsya,
                    Email = model.Email,
                    OperatorKabul = false,
                    Telno = model.Telno,
                    FaturaPath = model.FaturaPath,
                 
                };
                new ArizaKayitRepo().Insert(data);

                var LogMusteri = new ArizaLOG
                {
                    CreatedDate = DateTime.Now,
                    ArızaId = data.Id,
                    Aciklama = $"{data.Id}'nolu kayıt {userManager.Name} {userManager.Surname} İsimli Müşteri Tarafından Oluşturuldu.",
                    YapanınRolu = IdentityRoles.Musteri,
                };
                new ArizaLogRepo().Insert(LogMusteri);


                #region ArızaResimİşlemi
                if (model.PostedFileAriza.Count > 0)
                {
                    model.PostedFileAriza.ForEach(file =>
                    {
                        if (file != null && file.ContentLength > 0)
                        {

                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extName = Path.GetExtension(file.FileName);
                            fileName = StringHelpers.UrlFormatConverter(fileName);
                            fileName += StringHelpers.GetCode();
                            var klasoryolu = Server.MapPath("~/Ariza/");
                            var dosyayolu = Server.MapPath("~/Ariza/") + fileName + extName;

                            if (!Directory.Exists(klasoryolu))
                                Directory.CreateDirectory(klasoryolu);
                            file.SaveAs(dosyayolu);

                            WebImage img = new WebImage(dosyayolu);
                            img.Resize(250, 250, false);
                            img.Save(dosyayolu);

                            new FotografRepo().Insert(new Fotograf()
                            {
                                ArizaId = data.Id,
                                Yol = "/Ariza/" + fileName + extName
                            });
                        }
                    });
                }
                new ArizaKayitRepo().Update(data);

                #endregion


                var emailService = new EmailService();
                var body = $"Merhaba <b>{userManager.Name} {userManager.Surname}</b><br>Arıza Kaydınız Oluşturulmuştur. En kısa zamanda arızanız giderilicektir.<br>Fitech Mutlu Günler Diler.<br>  ";
                emailService.Send(new IdentityMessage() { Body = body, Subject = $"{userManager.UserName} Arıza Kaydı" }, model.Email);
                TempData["Message"] = $"{model.BeyazEsya} arıza şikayetiniz alınmıştır.";
                return RedirectToAction("Index");
              

            }
            catch (DbEntityValidationException ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {EntityHelpers.ValidationMessage(ex)}",
                    ActionName = "Index",
                    ControllerName = "Musteri",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Musteri",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        
        }

        [HttpGet]
        [Authorize]
        public ActionResult ArizaTakip()
        {
            //o an giriş yapan user'ın id'sini getirir.
            var musteriId = HttpContext.User.Identity.GetUserId();
            try
            {
                //arizalar tablosuna sistemde hangi müşteri giriş yapmıssa ona göre bir sorgu atılıyor. gelen sorguda olusturuldugu tarihe göre sıralanaıp sayfaya gönderiyoruz.
                var arizalar = new ArizaKayitRepo()
                    .GetAll(x => x.MusteriId == musteriId)
                    .OrderBy(x=>x.ArizaOlusturmaTarihi)
                    .ToList();
                return View(arizalar);
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {ex.Message}",
                    ActionName = "Index",
                    ControllerName = "Musteri",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }

    }
}