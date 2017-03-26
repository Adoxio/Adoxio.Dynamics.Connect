using System;
using System.Diagnostics;

namespace Adoxio.Dynamics.Connect
{
    public class S2SAppSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Resource { get; set; }

        public S2SAppSettings()
        { }

        public S2SAppSettings(string clientId, string clientSecret, string resource, string tenantId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Resource = resource;
            TenantId = tenantId;

            AssertAppSettings();
        }

        public void AssertAppSettings()
        {
            if (string.IsNullOrEmpty(ClientId) || !Guid.TryParse(ClientId, out Guid tempGuid))
            {
                throw new ArgumentException($"{nameof(ClientId)} is null or not valid format");
            }
            if (string.IsNullOrEmpty(ClientSecret))
            {
                throw new ArgumentNullException($"{nameof(ClientSecret)} is null");
            }
            if (!Uri.IsWellFormedUriString(Resource, UriKind.Absolute))
            {
                throw new UriFormatException($"{nameof(Resource)} is not well formed URI");
            }
            if (string.IsNullOrEmpty(TenantId) || !Guid.TryParse(TenantId, out tempGuid))
            {
                throw new ArgumentException($"{nameof(TenantId)} is null or not valid format");
            }
            Trace.TraceInformation("Successfully asserted S2S App Settings");
        }

        public bool ValidateAppSettings()
        {
            try
            {
                AssertAppSettings();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public virtual bool Equals(S2SAppSettings settings)
        {
            if (!ClientId.Equals(settings.ClientId))
                return false;
            if (!ClientSecret.Equals(settings.ClientSecret))
                return false;
            if (!Resource.Equals(settings.Resource))
                return false;
            if (!TenantId.Equals(settings.TenantId))
                return false;
            return true;
        }
    }
}
