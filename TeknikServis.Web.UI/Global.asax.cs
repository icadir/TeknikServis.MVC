﻿using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TeknikServis.BLL.Identity;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Web.UI.App_Start;
using System.Linq;
using TeknikServis.BLL.Helpers;

namespace TeknikServis.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMappings();
            var userManager = MembershipTools.NewUserManager();
            var userStore = MembershipTools.NewUserStore();
            var roller = Enum.GetNames(typeof(IdentityRoles));

            var roleManager = MembershipTools.NewRoleManager();
            foreach (var rol in roller)
            {
                if (!roleManager.RoleExists(rol))
                    roleManager.Create(new Role()
                    {
                       Name=rol,
                    });
            }
            if (!userStore.Users.Any())
            {
                DataHelper.DataEkle();
            }
        }
    }
}
