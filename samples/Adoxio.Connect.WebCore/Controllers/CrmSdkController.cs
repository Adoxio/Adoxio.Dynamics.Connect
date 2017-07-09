using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Adoxio.Connect.WebCore.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Diagnostics;

namespace Adoxio.Connect.WebCore.Controllers
{
    public class CrmSdkController : Controller
    {
        private readonly CrmContextCore _crmContext;

        public CrmSdkController(CrmContextCore crmContext)
        {
            _crmContext = crmContext;
        }

        public IActionResult Index()
        {
            var contacts = _crmContext.ServiceContext.CreateQuery("contact").ToList();
            return View(model: string.Join(",", contacts.Select(a => a.GetAttributeValue<string>("fullname"))));
        }

        public IActionResult WhoAmI()
        {
            WhoAmIResponse response = null;

            if (_crmContext?.WebProxyClient != null)
                response = (WhoAmIResponse)_crmContext.WebProxyClient.Execute(new WhoAmIRequest());

            string responseText = string.Empty;
            if (response != null)
                responseText = $"{response.UserId}";
            else
                responseText = $"WebProxyClient null";

            return View((object)responseText);
        }

        public IActionResult MultipleCalls()
        {
            Trace.TraceInformation("Start MultipleCalls");

            WhoAmIResponse response = null;

            if (_crmContext?.WebProxyClient != null)
            {
                response = (WhoAmIResponse)_crmContext.WebProxyClient.Execute(new WhoAmIRequest());
                Trace.TraceInformation("WhoAmI Executed");
            }

            string responseText = string.Empty;
            if (response != null)
                responseText = $"{response.UserId} : ";
            else
                responseText = $"WebProxyClient null : ";

            List<string> contacts = null;

            if (_crmContext?.ServiceContext != null)
            {
                contacts = _crmContext.ServiceContext.CreateQuery("contact").Select(a => a.GetAttributeValue<string>("fullname")).ToList();
                Trace.TraceInformation("Contact Query Executed");
            }

            responseText += contacts != null ? string.Join(",", contacts) : "null";

            return View((object)responseText);
        }

        [Produces("application/json")]
        [Route("api/CrmSdk")]
        public IEnumerable<Entity> GetContacts()
        {
            var contacts = _crmContext.ServiceContext.CreateQuery("contact").ToList();
            return contacts;
        }
    }
}