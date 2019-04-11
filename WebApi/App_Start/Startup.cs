using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApi.Models;

[assembly: OwinStartup(typeof(WebApi.App_Start.Startup))]

namespace WebApi.App_Start
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    HttpConfiguration configuration = new HttpConfiguration();
        //    ConfigureOAuth(app);
        //    //ayarlarımız..
        //    WebApiConfig.Register(configuration);
        //    app.UseWebApi(configuration);
        //}
        private void ConfigureOAuth(IAppBuilder app)
        {
            var oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(9999),
                AllowInsecureHttp = true,
                Provider = new Provider()
            };

            app.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
