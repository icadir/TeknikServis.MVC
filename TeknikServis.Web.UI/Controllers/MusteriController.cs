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
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.Controllers
{
    [Authorize(Roles = "Admin,Musteri")]
    public class MusteriController : Controller
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
            var MusteriId = HttpContext.User.Identity.GetUserId();

            //if (!ModelState.IsValid)
            //{
            //    //Gelen model valid degiilse bu sayfaya yönlendirilip hatalar gösterilicek.
            //    return RedirectToAction("Index", "Musteri", model);
            //}


            try
            {
                #region ArızaResimİşlemi
                if (model.PostedFileAriza != null &&
                    model.PostedFileAriza.ContentLength > 0)
                {
                    var file = model.PostedFileAriza;
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
                    img.AddTextWatermark("FİTech");
                    img.Save(dosyayolu);
                    var oldPath = model.ArızaPath;
                    model.ArızaPath = "/Ariza/" + fileName + extName;

                    System.IO.File.Delete(Server.MapPath(oldPath));
                }


                #endregion

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
                    MusteriId = MusteriId,
                    Adres = model.Adres,
                    ArizaDurumu = model.ArizaDurumu,
                    ArizaOlusturmaTarihi = DateTime.Now,
                    ArızaAcıklaması = model.ArızaAcıklaması,
                    BeyazEsya = model.BeyazEsya,
                    Email = model.Email,
                    OperatorKabul = false,
                    Telno = model.Telno,
                    ArızaPath = model.ArızaPath,
                    FaturaPath = model.FaturaPath,
                };
                new ArizaKayitRepo().Insert(data);
                TempData["Message"] = $"{model.ArizaId} no'lu kayıt başarıyla eklenmiştir";
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
            return View();
        }
    }
}