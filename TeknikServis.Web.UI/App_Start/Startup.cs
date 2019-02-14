using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

// OWIN START add new item dan ekleniyor. eklenmeden çalışmıyor. Ekledikten sonra kendisi yapıyor .
[assembly: OwinStartup(typeof(TeknikServis.Web.UI.App_Start.Startup))]

namespace TeknikServis.Web.UI.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //burası Owın eklendikten sonra yazılmalı.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

          
          
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account")
                });

               
               
            
        }
    }
}
