using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;

namespace Adoxio.Dynamics.Connect
{
    public class CrmContext : IDisposable
    {
        private const string ServiceEndpoint = @"/xrmservices/2011/organization.svc/web?SdkClientVersion=";
        private const string AadInstance = "https://login.microsoftonline.com/";

        private readonly string _sdkClientVersion = ConfigurationManager.AppSettings["dyn:SdkClientVersion"] ?? "8.2";
        private OrganizationWebProxyClient _organizationWebProxyClient;
        private OrganizationServiceContext _organizationServiceContext;
        private readonly string _resource;
        private string _token;
        private readonly Func<string, string> _getToken;
        private readonly SettingManager.S2SAppSettings _appSettings;

        public CrmContext()
        {
            _appSettings = SettingManager.GetAppSettings();
            Trace.TraceInformation("CrmContext set app settings");

            if (_appSettings == null)
            {
                throw new Exception("no app settings in config or settings.json");
            }

            _resource = _appSettings.Resource;
            Trace.TraceInformation($"CrmContext set instance: {_resource}");

            _getToken = GetDefaultToken;
        }

        private string GetDefaultToken(string resource)
        {
            var authority = $"{AadInstance}{_appSettings.TenantId}";
            var authContext = new AuthenticationContext(authority);
            Trace.TraceInformation($"CrmContext set auth context: {authority}");
            var result = authContext.AcquireToken(_appSettings.Resource,
                        new ClientCredential(_appSettings.ClientId, _appSettings.ClientSecret));
            Trace.TraceInformation($"ADAL AcquireToken complete");

            return result.AccessToken;
        }

        public CrmContext(string resource, string token)
        {
            _resource = resource;
            _token = token;
        }

        public CrmContext(string resource, Func<string, string> getToken = null)
        {
            _resource = resource;
            _getToken = getToken;
        }

        public OrganizationWebProxyClient WebProxyClient
        {
            get
            {
                Trace.TraceInformation("Get WebProxyClient");
                // create client for instance if null
                if (_organizationWebProxyClient == null)
                {
                    Trace.TraceInformation("WebProxyClient is null");
                    var serviceUri = new Uri($"{_resource}{ServiceEndpoint}{_sdkClientVersion}");
                    _organizationWebProxyClient = new OrganizationWebProxyClient(serviceUri, false);
                    Trace.TraceInformation($"Init OrganizationWebProxyClient: {serviceUri}");
                }

                var token = _token;
                // get token if token string is empty
                if (string.IsNullOrEmpty(token))
                {
                    Trace.TraceInformation("Execute GetToken");
                    token = _getToken(_resource);
                }

                // add latest token token to client header
                _organizationWebProxyClient.HeaderToken = token;
                Trace.TraceInformation("token added to WebProxyClient headertoken");
                return _organizationWebProxyClient;
            }
        }

        public OrganizationServiceContext ServiceContext
        {
            get
            {
                Trace.TraceInformation("Get ServiceContext");
                if (_organizationServiceContext == null)
                {
                    Trace.TraceInformation("ServiceContext is null");
                    _organizationServiceContext = new OrganizationServiceContext(WebProxyClient);
                }

                return _organizationServiceContext;
            }
        }

        public static CrmContext Create()
        {
            return new CrmContext();
        }

        public void Dispose()
        {
            _organizationWebProxyClient?.Dispose();
            _organizationServiceContext?.Dispose();
            Trace.TraceInformation("WebProxyClient and ServiceContext disposed");
        }
    }
}