using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Services
{
    public class LocalizationService : ILocalizationService
    {
        readonly CultureInfo ci;
        const string ResourceId = "Pollenalarm.Frontend.Forms.Resources.Strings";

        public LocalizationService()
        {
            ci = DependencyService.Get<ICultureService>().GetCurrentCultureInfo();
        }

        public string GetString(string key)
        {
            ResourceManager resmgr = new ResourceManager(ResourceId, typeof(LocalizationService).GetTypeInfo().Assembly);
            var translation = resmgr.GetString(key, ci);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", key, ResourceId, ci.Name),
                    "Text");
#else
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
