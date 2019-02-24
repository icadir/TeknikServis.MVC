using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Identity;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using static TeknikServis.BLL.Identity.MembershipTools;
namespace TeknikServis.Web.UI.Controllers
{
    [Authorize]

    public class BaseController : Controller
    {
        // GET: Base
        private List<User> Teknisyenler = new List<User>();
        protected List<SelectListItem> GetRoleList()
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

        protected List<SelectListItem> BostaOlanTeknisyenler()
        {
            List<SelectListItem> Teknisyenler = new List<SelectListItem>();
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

            return Teknisyenler;
        }
    }
}