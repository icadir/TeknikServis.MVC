using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.ViewModels;
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
    }
}