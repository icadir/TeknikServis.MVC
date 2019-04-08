using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels.ArizaViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;
namespace TeknikServis.Web.UI.Controllers
{
    [Authorize]

    public class BaseController : Controller
    {
        // GET: Base
        private List<User> Teknisyenler = new List<User>();
        public List<SelectListItem> GetRoleList()
        {
            var data = new List<SelectListItem>();
            MembershipTools.NewRoleStore().Roles
                .ToList()
                .ForEach(x =>
                {
                    data.Add(new SelectListItem()
                    {
                        Text = $"{x.Name}",
                        Value = x.Id
                    });
                });
            return data;
        }

        public List<SelectListItem> BostaOlanTeknisyenler(ArizaViewModel model)
        {
            List<SelectListItem> Teknisyenler = new List<SelectListItem>();
            //Tüm Teknisyenleri çekiyorm.
            var RoleTeknisyenler = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();

          
         

            //ariza tablosuna bir alan ekledim teknisyen durum çalışıyor,hasta,botagibi. üst tarafta bütün teknisyenleri çakiyorum. aşagıda bu teknisyenleri gezerek bir sorgu atıyorum herseferinde . eger bu teknisn ıdsındeki kişi arıza tablomda var Ve oldugu tabloda çalışıyor duumda ise bu teknisyen çalışıyordur diyip. geçiyorum . ama eger null geliyorsa bu teknisyen çalışmıyordur diyorum ve ekliyorum.


            for (int i = 0; i < RoleTeknisyenler.Count; i++)
            {
               
                    var distance = 0.0;
                    var distanceString = 0.0;
                    var technician = NewUserManager().FindById(RoleTeknisyenler[i]);
                    if (technician.Enlem.HasValue && technician.Boylam.HasValue && model.Enlem.HasValue && model.Boylam.HasValue)
                    {
                        var failureCoordinate = new GeoCoordinate(model.Enlem.Value, model.Boylam.Value);
                        var technicianCoordinate = new GeoCoordinate(technician.Enlem.Value, technician.Boylam.Value);

                        distance = failureCoordinate.GetDistanceTo(technicianCoordinate);
                        distanceString = distance / 1000 ;
  

                    }

                    var calisiyormu = new ArizaKayitRepo().GetAll().FirstOrDefault(x => x.TeknisyenId == RoleTeknisyenler[i] && x.TeknisyenDurumu == TeknisyenDurumu.Calısıyor);

                if (calisiyormu == null)
                {
                
                    Teknisyenler.Add(new SelectListItem()
                    {
                        //TODO User alanına veya teknisyenlere özel olarak uzmanlık vs nasıl eklenebilir soralım hocaya eger olursa buarada combo boxta uzmanlık alanını getiririz. zaten İşi varmı yokmuyu nasıl ekliceksek aynı mantık olabilir.
                        Text = technician.Name + " " + technician.Surname+" "+"Arızaya Olan Mesafe"+ " " + distanceString.ToString("##.000") + "Km",
                        Value = technician.Id
                    });

                }

            }

            return Teknisyenler;
        }
    }
}