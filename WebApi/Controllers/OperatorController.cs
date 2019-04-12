using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using TeknikServis.BLL.Repository;
using TeknikServis.BLL.Services.Senders;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.Models;
using TeknikServis.Entity.ViewModels.ApiViewModel.Operator;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

namespace WebApi.Controllers
{
   

    [EnableCors("*", "*", "*")]
    [Authorize]
    public class OperatorController : ApiController
    {
        //Not found vs buradaIHHTp actionda mesaj ile göndermeli yapı yokmu. 
        // idleri ipde bulup gönderemiyoruz. veya bir kayıt eklerken idleri nasıl alcaz
     
        [HttpGet]
        public async Task<IHttpActionResult> Index()
        {
            List<OperatorIndexViewModel> data = new List<OperatorIndexViewModel>();
            try
            {
                var arizalist = new ArizaKayitRepo()
                    .GetAll(x => x.OperatorKabul == false)
                    //.Select(x => Mapper.Map<ArizaViewModel>(x))
                    .ToList();
                for (int i = 0; i < arizalist.Count(); i++)
                {
                    arizalist[i].ArızaPath = new FotografRepo().GetAll(z => z.ArizaId == arizalist[i].Id).Select(u => u.Yol).ToList();
                }
                //Suanda burada bir kulşlanıcı yokki tabi gelmez.
                var SistemdekiId = HttpContext.Current.User.Identity.GetUserId();
                for (int i = 0; i < arizalist.Count(); i++)
                {

                    data.Add(new OperatorIndexViewModel
                    {
                        FaturaResim = arizalist[i].FaturaPath,
                        Adres = arizalist[i].Adres,
                        ArizaCreatedDate = arizalist[i].CreatedDate,
                        ArızaId = arizalist[i].Id,
                        MusteriId = arizalist[i].MusteriId,
                        ArızaResim = arizalist[i].ArızaPath,
                        Sistemdekiteknisyen = SistemdekiId,
                        telno = arizalist[i].Telno,

                    });
                }

                return Ok(new ResponseData()
                {
                    success = true,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> ArizaDetay(int id)
        {
            try
            {

                var ariza = new ArizaKayitRepo().GetById(id);
                // Her list olayını için böylemi yapmalı
                var fotolar = new FotografRepo().GetAll(x => x.ArizaId == ariza.Id).Select(x => x.Yol).ToList();

                ariza.ArızaPath = fotolar;

                var data = new OperatorIndexViewModel
                {
                    ArızaResim = ariza.ArızaPath,
                    Adres = ariza.Adres,
                    ArızaId = ariza.Id,
                    MusteriId = ariza.MusteriId,
                    FaturaResim = ariza.FaturaPath,
                    telno = ariza.Telno,
                    ArizaCreatedDate = ariza.CreatedDate,

                };

                //arizadetaydata.ArızaPath = new FotografRepo().GetAll(z => z.ArizaId == id).Select(u => u.Yol).ToList();

                if (data != null)
                {
                    return Ok(new ResponseData()
                    {
                        success = true,
                        data = data
                    });
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IHttpActionResult> ArizaKabul(int id)
        {
            // Nasıl alınır opId bilmiyorm.
            var OpertatorId = HttpContext.Current.User.Identity.GetUserId();

            try
            {
                var ariza = await new ArizaKayitRepo().GetByIdAsync(id);
                var Operator = await NewUserManager().FindByIdAsync(OpertatorId);
                if (ariza == null)
                {

                    return Ok(new ResponseData
                    {
                        message = $"Kayıt Bulunamadı",
                        success = false,
                    });
                }
                else
                {
                    ariza.OperatorKabulTarih = DateTime.Now;
                    ariza.OperatorKabul = true;
                    ariza.OperatorId = OpertatorId;
                    ariza.ArizaDurumu = ArizaDurum.OperatorTakibeAldı;
                    var updatesonuc = new ArizaKayitRepo().Update(ariza);
                    if (updatesonuc < 1)
                    {
                        return Ok(new ResponseData
                        {
                            message = $"Ariza Kayit Başarısz",
                            success = false,
                        });
                    }

                    var OperatorLog = new ArizaLOG
                    {
                        CreatedDate = DateTime.Now,
                        ArızaId = id,
                        Aciklama =
                            $"Ariza'nız {Operator.Name} {Operator.Surname} isimli Operator Tarafından Onaylanmıştır.",
                        YapanınRolu = IdentityRoles.Teknisyen
                    };
                    var responce = new ArizaLogRepo().Insert(OperatorLog);
                    if (responce < 1)
                    {
                        return Ok(new ResponseData
                        {
                            message = $"ArızaLog Başarısz",
                            success = false,
                        });
                    }
                    return Ok(new ResponseData
                    {
                        message = $"Atama ve log işlemleri başaarılı",
                        success = true,
                    });
                    //TODO Müşteriye Mail gönderilir bilgilendirme belki
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }
        }

        //burası ayrı bir sayfa tek sayfa olarak yapmıstık onu deniyelim yapamazsak burayı kullanarak ayırırız sayfaları deneriz.
        [HttpGet]
        public IHttpActionResult ArizaList()
        {
            //operator bulunuyor ve o operatorun aldıgı kayıtlar listelenip çekiliyor.

            var OpertatorId = HttpContext.Current.User.Identity.GetUserId();
            try
            {
                var data = new ArizaKayitRepo()
                    .GetAll(x => x.OperatorId == OpertatorId)
                    .Select(x => Mapper.Map<ArizaViewModel>(x))
                    .OrderBy(x => x.ArizaOlusturmaTarihi)
                    .ToList();
                return Ok(new ResponseData()
                {
                    success = true,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata olustu{ex.Message}");
            }
        }

        [HttpPost]
        //TODO TEknisyen atamayı burada yap
        public async Task<IHttpActionResult> TeknisyenAta(ArizaViewModel model)
        {

            try
            {

                var ariza = new ArizaKayitRepo().GetById(model.ArizaId);
                ariza.TeknisyenId = model.UserId;
                ariza.ArizaDurumu = ArizaDurum.TeknisyenAtandi;
                ariza.TeknisyenDurumu = TeknisyenDurumu.Calısıyor;
                ariza.TeknisyenAtandigiTarih = DateTime.Now;
                var responce = new ArizaKayitRepo().Update(ariza);
                if (responce < 1)
                {
                    return Ok(new ResponseData
                    {
                        message = $"Ariza Kayıt Update Edilemedi.",
                        success = false,
                    });
                }

                var opId = HttpContext.Current.User.Identity.GetUserId();
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
                var Logresponce = new ArizaLogRepo().Insert(OperatorLog);

                if (Logresponce < 1)
                {
                    return Ok(new ResponseData
                    {
                        message = $"Log kayıt Eklenemedi",
                        success = false,
                    });

                }

                string SiteUrl = Request.RequestUri.Scheme + Uri.SchemeDelimiter + Request.RequestUri.Host +
                                 (Request.RequestUri.IsDefaultPort ? "" : ":" + Request.RequestUri.Port);
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

                return Ok(new ResponseData()
                {
                    success = true,
                    message = $"Tüm işlemler başarılı bir şekilde gerçekleştri"

                });
            }

            catch (Exception ex)
            {
                return BadRequest($"Bir hata olustu{ex.Message}");
            }

        }
        [HttpGet]
        public async Task<IHttpActionResult> OPArizaDetay(int id)
        {

            try
            {
                var ariza = new ArizaKayitRepo().GetById(id);
                var fotolar = new FotografRepo().GetAll(x => x.ArizaId == ariza.Id).Select(x => x.Yol).ToList();
                ariza.ArızaPath = fotolar;
                var data = new OperatorIndexViewModel
                {
                    ArızaResim = ariza.ArızaPath,
                    Adres = ariza.Adres,
                    ArızaId = ariza.Id,
                    MusteriId = ariza.MusteriId,
                    FaturaResim = ariza.FaturaPath,
                    telno = ariza.Telno,
                    ArizaCreatedDate = ariza.CreatedDate,

                };


                List<BosTeknisyenViewModel> TeknisyenList = new List<BosTeknisyenViewModel>();
                var RoleTeknisyenler = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();

                for (int i = 0; i < RoleTeknisyenler.Count; i++)
                {

                    //var distance = 0.0;
                    var distanceString = 0.0;
                    var technician = NewUserManager().FindById(RoleTeknisyenler[i]);
                    //if (technician.Enlem.HasValue && technician.Boylam.HasValue && model.Enlem.HasValue && model.Boylam.HasValue)
                    //{
                    //    var failureCoordinate = new GeoCoordinate(model.Enlem.Value, model.Boylam.Value);
                    //    var technicianCoordinate = new GeoCoordinate(technician.Enlem.Value, technician.Boylam.Value);

                    //    distance = failureCoordinate.GetDistanceTo(technicianCoordinate);
                    //    distanceString = distance / 1000;


                    //}

                    var calisiyormu = new ArizaKayitRepo().GetAll().FirstOrDefault(x =>
                        x.TeknisyenId == RoleTeknisyenler[i] && x.TeknisyenDurumu == TeknisyenDurumu.Calısıyor);

                    if (calisiyormu == null)
                    {

                        TeknisyenList.Add(new BosTeknisyenViewModel()
                        {
                            Text = technician.Name + " " + technician.Surname + " " + "Arızaya Olan Mesafe" + " " +
                                   distanceString.ToString("##.000") + "Km",
                            Value = technician.Id
                        });

                    }
                }

                return Ok(new ResponseData()
                {
                    data = data,
                    success = true,
                    Teknisyenler = TeknisyenList
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata olustu{ex.Message}");
            }

        }
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var user = await NewUserManager().FindByIdAsync(id);
            if (user == null)
            {
                return Ok(new ResponseData
                {
                    message = "Bir hata olustu",
                    success = false,

                });
            }
            else
            {
                return Ok(new ResponseData
                {
                    data = user,
                    success = true,

                });
            }
           
        }

    }
}
