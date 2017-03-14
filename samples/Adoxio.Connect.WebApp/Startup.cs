using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Adoxio.Dynamics.Connect;

[assembly: OwinStartup(typeof(Adoxio.Connect.WebApp.Startup))]

namespace Adoxio.Connect.WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            if (!SettingManager.InitAppSettings())
            {
                throw new Exception("no Adoxio Connect settings found");
            }
            else
            {
                app.CreatePerOwinContext<CrmContext>(CrmContext.Create);
            }
        }
    }
}
