using Adoxio.Dynamics.Connect;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adoxio.Connect.WebApp.Controllers
{
    public class CrmSdkController : Controller
    {
        // GET: CrmSdk
        public ActionResult Index()
        {
            var context = Request.GetOwinContext().Get<CrmContext>();

            WhoAmIResponse response = null;

            if (context != null)
            {
                response = (WhoAmIResponse) context.WebProxyClient.Execute(new WhoAmIRequest());
            }

            return View((object)string.Join(",", response.Results.ToList()));
        }
    }
}