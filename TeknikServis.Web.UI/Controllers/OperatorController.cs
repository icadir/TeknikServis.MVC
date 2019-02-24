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
    public class OperatorController : Controller
    {
        // GET: Operator
        public ActionResult Index()
        {

            var data = new ArizaKayitRepo()
                .GetAll(x => x.OperatorKabul == false)
                .Select(x => Mapper.Map<ArizaViewModel>(x))
                .ToList();
            //    .Select(x => new ArizaViewModel()
            //{
            //    Adres = x.Adres,
            //    AnketYapildimi = x.AnketYapildimi,
            //    ArizaDurumu = x.ArizaDurumu,
            //    ArizaCozulduguTarih = x.ArizaCozulduguTarih,
            //    ArizaId = x.Id,
            //    ArizaOlusturmaTarihi = x.ArizaOlusturmaTarihi,
            //    ArızaAcıklaması = x.ArızaAcıklaması,
            //    ArizaSonKontrolTarihi = x.ArizaSonKontrolTarihi,
            //    BeyazEsya = x.BeyazEsya,
            //    Email = x.Email,
            //    MusteriId = x.MusteriId,
            //    OperatorId = x.OperatorId,
            //    OperatorKabulTarih = x.OperatorKabulTarih,
            //    Telno = x.Telno,
            //    TeknisyenArizaAciklama = x.TeknisyenArizaAciklama,
            //    TeknisyenAtandigiTarih = x.TeknisyenAtandigiTarih,
            //    TeknisyenId = x.TeknisyenId,
            //    TeknisyenIstemi = x.TeknisyenIstemi,
            //    OperatorKabul = x.OperatorKabul,
            //    Boylam = x.Boylam,
            //    Enlem = x.Enlem,
            //    FaturaPath = x.FaturaPath,

            //})
            return View(data);
        }

        [HttpGet]
        public async Task<ActionResult> ArizaDetay(int id)
        {
            try
            {

                var x = new ArizaKayitRepo().GetById(id);

                var data = Mapper.Map<ArizaViewModel>(x);
                data.ArızaPath = x.Fotograflar.Select(y => y.Yol).ToList();
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
                    RedirectToAction("ArizaList", "Operator");
                    //TODO Müşteriye Mail gönderilir bilgilendirme belki
                }

                return RedirectToAction("ArizaList", "Operator");

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


        List<SelectListItem> Teknisyenler = new List<SelectListItem>();
        public ActionResult OPArizaDetay(int id)
        {

            try
            {

                //TODO Burayı Base kontrollıra Tası .
                //TODO Projede kaldıgınyer ArizaList sayfasında teknisyen ata diyince gelen ekranda teknisyen seçip Atama işlemini yapıcaksın. Teknisyen sayfası yapıcaksın . teknisyen atandıgı yerde müsteriye ve teknisyene mail atıcaksın.
                //TODO User alanına veya teknisyenlere özel olarak uzmanlık vs nasıl eklenebilir soralım hocaya eger olursa buarada combo boxta uzmanlık alanını getiririz. zaten

                //Tüm Teknisyenleri çekiyorm.
                var RoleTeknisyenler = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();

               //ariza tablosuna bir alan ekledim teknisyen durum çalışıyor,hasta,botagibi. üst tarafta bütün teknisyenleri çakiyorum. aşagıda bu teknisyenleri gezerek bir sorgu atıyorum herseferinde . eger bu teknisn ıdsındeki kişi arıza tablomda var Ve oldugu tabloda çalışıyor duumda ise bu teknisyen çalışıyordur diyip. geçiyorum . ama eger null geliyorsa bu teknisyen çalışmıyordur diyorum ve ekliyorum.


                for (int i = 0; i < RoleTeknisyenler.Count; i++)
                {
                    var calisiyormu = new ArizaKayitRepo().GetAll().FirstOrDefault(x => x.TeknisyenId == RoleTeknisyenler[i] && x.TeknisyenDurumu == TeknisyenDurumu.Calısıyor);

                    if (calisiyormu == null)
                    {
                        var User = NewUserManager().FindById(RoleTeknisyenler[i]);

                    
                            Teknisyenler.Add(new SelectListItem()
                            {
                                //TODO User alanına veya teknisyenlere özel olarak uzmanlık vs nasıl eklenebilir soralım hocaya eger olursa buarada combo boxta uzmanlık alanını getiririz. zaten İşi varmı yokmuyu nasıl ekliceksek aynı mantık olabilir.
                                Text = User.Name + " " + User.Surname,
                                Value = User.Id
                            });
                        
                    }


                }


                ViewBag.TeknisyenK = Teknisyenler;
                var data = new ArizaKayitRepo()
                    .GetAll(x => x.Id == id)
                    .Select(x => Mapper.Map<ArizaViewModel>(x)).FirstOrDefault();
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