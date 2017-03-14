using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Adoxio.Dynamics.Connect
{
    public static class CrmContextExtensions
    {
        public static Entity GetContactByExternalIdentityUsername(this CrmContext context, string username)
        {
            var fetchxml =
                $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' nolock='true'>
                      <entity name='contact'>
                        <all-attributes />
                        <link-entity name='adx_externalidentity' from='adx_contactid' to='contactid' alias='ab'>
                          <filter type='and'>
                            <condition attribute='adx_username' operator='eq' value='{username}' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

            var contactResponse = context.WebProxyClient.RetrieveMultiple(new FetchExpression(fetchxml));

            return contactResponse.Entities.SingleOrDefault();
        }
    }
}
