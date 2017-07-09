using Adoxio.Dynamics.Connect;
using Microsoft.Extensions.Options;

namespace Adoxio.Connect.WebCore.Extensions
{
    public class CrmContextCore : CrmContext
    {
        public CrmContextCore(IOptions<S2SAppSettings> s2sAppSettingsOptions) : base(s2sAppSettingsOptions.Value)
        {
        }
    }
}
