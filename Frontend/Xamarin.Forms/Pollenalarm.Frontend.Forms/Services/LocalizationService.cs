using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly CultureInfo cultureInfo;
        const string ResourceId = "Pollenalarm.Frontend.Forms.Resources.Strings";

        public LocalizationService()
        {
            cultureInfo = DependencyService.Get<ICultureService>().GetCurrentCultureInfo();
        }

        public string GetString(string key)
        {
            ResourceManager resmgr = new ResourceManager(ResourceId, typeof(LocalizationService).GetTypeInfo().Assembly);
            var translation = resmgr.GetString(key, cultureInfo);

            if (translation == null)
            {
                Debug.WriteLine($"Key '{key}' was not found in resources '{ResourceId}' for culture '{cultureInfo.Name}'.");
                translation = key;
            }

            return translation;
        }
    }
}
