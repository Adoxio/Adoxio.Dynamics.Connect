using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using Newtonsoft.Json;

namespace Adoxio.Dynamics.Connect
{
    public static class SettingManager
    {
        public class S2SAppSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string TenantId { get; set; }
            public string Resource { get; set; }
        }

        private static readonly string _settingClientId = "dyn:ClientId";
        private static readonly string _settingClientSecret = "dyn:ClientSecret";
        private static readonly string _settingResource = "dyn:Resource";
        private static readonly string _settingTenantId = "dyn:TenantId";

        private static readonly Lazy<string> SettingsPath = new Lazy<string>(GetSettingsPath);
        private static readonly string[] MachineKeyPurposes = { "adoxio", "setup" };

        public static void Save(string clientId, string clientSecret, string tenantId, string resource)
        {
            var protectedSecret = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(clientSecret), MachineKeyPurposes));

            var jsonObject = new
            {
                ClientId = clientId,
                ClientSecret = protectedSecret,
                TenantId = tenantId,
                Resource = resource
            };

            using (var fs = File.Open(SettingsPath.Value, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, jsonObject);
                }
            }
        }

        public static bool Exists()
        {
            return File.Exists(SettingsPath.Value);
        }

        public static S2SAppSettings Read()
        {
            using (StreamReader r = new StreamReader(SettingsPath.Value))
            {
                string json = r.ReadToEnd();
                var settings = JsonConvert.DeserializeObject<S2SAppSettings>(json);

                if (!string.IsNullOrEmpty(settings.ClientSecret))
                {
                    var protectedSecret = settings.ClientSecret;
                    settings.ClientSecret = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(protectedSecret), MachineKeyPurposes));
                }

                return settings;
            }
        }

        public static bool InitAppSettings()
        {
            if (ValidateAppSettings(GetAppSettings()))
            {
                return true;
            }

            if (Exists() && LoadConfigAppSettings(Read()))
            {
                return true;
            }

            return false;
        }

        private static bool ValidateAppSettings(S2SAppSettings appSettings)
        {
            if (string.IsNullOrEmpty(appSettings.ClientId))
            {
                return false;
            }
            if (string.IsNullOrEmpty(appSettings.ClientSecret))
            {
                return false;
            }
            if (string.IsNullOrEmpty(appSettings.Resource))
            {
                return false;
            }
            if (string.IsNullOrEmpty(appSettings.TenantId))
            {
                return false;
            }
            return true;
        }

        public static bool LoadConfigAppSettings(S2SAppSettings appSettings)
        {
            try
            {
                ConfigurationManager.AppSettings[_settingClientId] = appSettings.ClientId;
                ConfigurationManager.AppSettings[_settingClientSecret] = appSettings.ClientSecret;
                ConfigurationManager.AppSettings[_settingResource] = appSettings.Resource;
                ConfigurationManager.AppSettings[_settingTenantId] = appSettings.TenantId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static S2SAppSettings GetAppSettings()
        {
            Trace.TraceInformation("GetAppSettings with ConfigurationManager");
            var settings = new S2SAppSettings()
            {
                ClientId = ConfigurationManager.AppSettings[_settingClientId],
                ClientSecret = ConfigurationManager.AppSettings[_settingClientSecret],
                Resource = ConfigurationManager.AppSettings[_settingResource],
                TenantId = ConfigurationManager.AppSettings[_settingTenantId]
            };

            return settings;
        }

        private static string GetSettingsPath()
        {
            var virtualPath = ConfigurationManager.AppSettings["SettingsPath"] ?? "~/App_Data/settings.json";
            var settingsPath = HostingEnvironment.MapPath(virtualPath);
            var dataDirectory = Path.GetDirectoryName(settingsPath);

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            return settingsPath;
        }
    }
}
