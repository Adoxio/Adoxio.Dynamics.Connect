using Adoxio.Dynamics.Connect;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Adoxio.Connect.WebApp.Controllers
{
    public class CrmSdkApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Contacts()
        {
            var context = HttpContext.Current.GetOwinContext().Get<CrmContext>();

            var contacts = context.ServiceContext.CreateQuery("contact").Select(a => a.GetAttributeValue<string>("fullname")).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, contacts);

        }
    }
}
