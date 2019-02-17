using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TeknikServis.BLL.Identity;
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

        //protected List<User> GetTeknisyen()
        //{

        //    var asdasd = NewRoleManager().FindByName("Teknisyen").Users.Select(x => x.UserId).ToList();
        //    for (int i = 0; i < asdasd.Count; i++)
        //    {
        //        var zzzzzz = NewUserManager().FindById(asdasd[i]);
        //        Teknisyenler.Add(zzzzzz);
        //    }

        //    return Teknisyenler;
        //}
    }
}