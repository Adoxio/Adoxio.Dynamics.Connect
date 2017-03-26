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
        private readonly Func<S2SAppSettings, string> _getToken;
        private static S2SAppSettings _appSettings;
        private static AuthenticationContext _authContext;

        public CrmContext() : this(SettingManager.GetAppSettings()) { }

        public CrmContext(string clientId, string clientSecret, string resource, string tenantId, Func<S2SAppSettings, string> getToken = null) 
            : this(new S2SAppSettings()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Resource = resource,
                    TenantId = tenantId
                }, getToken)
        { }

        public CrmContext(S2SAppSettings settings, Func<S2SAppSettings, string> getToken = null)
        {
            if (_appSettings == null || !_appSettings.Equals(settings))
            {
                settings.AssertAppSettings();
            }
            _appSettings = settings;

            _resource = settings.Resource;
            Trace.TraceInformation($"CrmContext set instance: {_resource}");

            _getToken = getToken == null ? GetDefaultToken : getToken;
        }

        public CrmContext(string resource, string token)
        {
            _resource = resource;
            _token = token;
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
                    token = _getToken(_appSettings);
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

        public static CrmContext Create(string resource, string clientId, string clientSecret, string tenantId)
        {
            return new CrmContext(resource, clientId, clientSecret, tenantId);
        }

        public static CrmContext Create(S2SAppSettings settings)
        {
            return new CrmContext(settings);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            Trace.TraceInformation("CrmContext dispose complete");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _organizationWebProxyClient?.Dispose();
                _organizationServiceContext?.Dispose();
                Trace.TraceInformation("WebProxyClient and ServiceContext disposed");
            }
        }

        public void ResetTokenCache()
        {
            if (_authContext != null)
            {
                _authContext.TokenCache.Clear();
                Trace.TraceInformation("ADAL TokenCache cleared");
            }
        }

        internal void ResetAll()
        {
            ResetTokenCache();
            _appSettings = null;
            _authContext = null;
            Trace.TraceInformation("Reset complete");
        }

        #region private helpers

        private string GetDefaultToken(S2SAppSettings settings)
        {
            var authority = $"{AadInstance}{settings.TenantId}";

            if (_authContext == null || !_authContext.Authority.Contains(authority))
            {
                _authContext = new AuthenticationContext(authority);
                Trace.TraceInformation($"CrmContext set auth context: {authority}");
            }
            var result = _authContext.AcquireToken(settings.Resource,
                        new ClientCredential(settings.ClientId, settings.ClientSecret));
            Trace.TraceInformation($"ADAL AcquireToken complete");

            return result.AccessToken;
        }

        #endregion
    }
}