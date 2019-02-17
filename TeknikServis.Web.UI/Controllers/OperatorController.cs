using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;


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
        public async Task<ActionResult> TeknisyenAta(ArizaViewModel model)
        {
            //TODO NOT Eger bir yerlerde program view modelden patlarsa string birşeyler EKledik ondandır.
            try
            {
                //TODO Teknisyen atandıgı tarihde eklenebilir istenirse.
                var ariza = new ArizaKayitRepo().GetById(model.ArizaId);
                ariza.TeknisyenId = model.UserId;
                ariza.ArizaDurumu = ArizaDurum.TeknisyenAtandi;
                new ArizaKayitRepo().Update(ariza);
               var teknisyen = await NewUserStore().FindByIdAsync(ariza.TeknisyenId);
               //TODO Musteriye ve Teknisyene mail gönder. 
                TempData["Message"] = $"{ariza.Id} nolu arızaya {teknisyen.Name}  {teknisyen.Surname} atanmıştır.İyi çalışmalar.";
                
              return  RedirectToAction("Index", "Operator");
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


        List<SelectListItem> Teknisyenler = new List<SelectListItem>();
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
                //TODO User alanına veya teknisyenlere özel olarak uzmanlık vs nasıl eklenebilir soralım hocaya eger olursa buarada combo boxta uzmanlık alanını getiririz. zaten
                var RoleTeknisyenler = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();
                    for (int i = 0; i < RoleTeknisyenler.Count; i++)
                    {
                        var User = NewUserManager().FindById(RoleTeknisyenler[i]);
                        Teknisyenler.Add(new SelectListItem()
                        {
                            //TODO User alanına veya teknisyenlere özel olarak uzmanlık vs nasıl eklenebilir soralım hocaya eger olursa buarada combo boxta uzmanlık alanını getiririz. zaten İşi varmı yokmuyu nasıl ekliceksek aynı mantık olabilir.
                            Text = User.Name + " "+ User.Surname,
                            Value = User.Id
                        });        
                    }

                    ViewBag.TeknisyenK = Teknisyenler;         
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