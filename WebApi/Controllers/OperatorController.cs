using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.ViewModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

namespace WebApi.Controllers
{
    public class OperatorController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var arizalist = new ArizaKayitRepo()
                    .GetAll(x => x.OperatorKabul == false)
                    .Select(x => Mapper.Map<ArizaViewModel>(x))
                    .ToList();
                if (arizalist != null)
                {
                    return Ok(new
                    {
                        success = true,
                        data = arizalist,
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
        [HttpGet]
        public async Task<IHttpActionResult> ArizaDetay(int id)
        {
            try
            {

                var x = new ArizaKayitRepo().GetById(id);

                var arizadetaydata = Mapper.Map<ArizaViewModel>(x);
                arizadetaydata.ArızaPath = new FotografRepo().GetAll(z => z.ArizaId == id).Select(u => u.Yol).ToList();
                if (arizadetaydata != null)
                {
                    return Ok(new
                    {
                        succes = true,
                        data = arizadetaydata
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

        public async Task<IHttpActionResult> ArizaKabul(int id)
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
    }
}
