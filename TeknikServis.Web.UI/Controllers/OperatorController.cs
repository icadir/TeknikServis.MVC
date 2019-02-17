using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;
using TeknikServis.Entity.ViewModels.ArizaViewModels;


namespace TeknikServis.Web.UI.Controllers
{
    public class OperatorController : Controller
    {
        // GET: Operator
        public ActionResult Index()
        {
            // TODO mapperları yapalım.
            var data = new ArizaKayitRepo().GetAll(x => x.OperatorKabul == false).Select(x => Mapper.Map<ArizaViewModel>(x)).ToList();

            return View(data);
        }

        [HttpGet]
        public async Task<ActionResult> ArizaDetay(int id)
        {
            try
            {
                var x = await new ArizaKayitRepo().GetByIdAsync(id);
                var data = Mapper.Map<ArizaViewModel>(x);
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
                var ariza =await new ArizaKayitRepo().GetByIdAsync(id);
                if (ariza==null)
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
                    RedirectToAction("Index", "Operator");
                    //TODO Müşteriye Mail gönderilir bilgilendirme belki
                }

                return RedirectToAction("Index","Operator");

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
                    .Select(x=> Mapper.Map<ArizaViewModel>(x))
                    .OrderBy(x=>x.ArizaOlusturmaTarihi)
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
        public ActionResult TeknisyenAta(User model)
        {
            try
            {

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
            return View();
        }


        List<User> Teknisyenler = new List<User>();
        List<SelectListItem> TeknisyenX = new List<SelectListItem>();
        public ActionResult OPArizaDetay(int id)
        {
           
            try
            {
                //TODO Beyin yanması  ve yapılacaklar .
                //TODO Burayı Base kontrollıra Tası .
                //TODO Hocaya Dropdownlistfor u sor . Id olarak nasıl  çekeriz sor. Sonra buradaki gereksizleri temizle.
                //TODO Projede kaldıgınyer ArizaList sayfasında teknisyen ata diyince gelen ekranda teknisyen seçip Atama işlemini yapıcaksın. Teknisyen sayfası yapıcaksın . teknisyen atandıgı yerde müsteriye ve teknisyene mail atıcaksın.
                //TODO Teknisyen için  İşi var İşi yok nasıl kontrol edilir sor. Yani Users Tablosuna Bir Alan mı eklenicek Bool olarak teknisyen atanırken o userın işi var gelicek ? 
                //TODO HOCAYI BIRAKMA HEPSİNİ SORRRRr :D
                var asdasd = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();
                    for (int i = 0; i < asdasd.Count; i++)
                    {
                        var zzzzzz = NewUserManager().FindById(asdasd[i]);
                        TeknisyenX.Add(new SelectListItem()
                        {
                            Text = zzzzzz.Name,
                            Value = zzzzzz.Id
                        });
                   Teknisyenler.Add(zzzzzz);
                    
                    }

                    ViewBag.TeknisyenK = TeknisyenX;
                    ViewBag.Teknisyenler = new SelectList(Teknisyenler, "Id", "Name");
               
                var data = new ArizaKayitRepo()
                    .GetAll(x=>x.Id==id)
                    .Select(x=>Mapper.Map<ArizaViewModel>(x)).FirstOrDefault();
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