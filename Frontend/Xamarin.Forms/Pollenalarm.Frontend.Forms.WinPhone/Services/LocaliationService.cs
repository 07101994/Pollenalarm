using Pollenalarm.Frontend.Forms.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pollenalarm.Frontend.Forms.WinPhone.Services.LocaliationService))]

namespace Pollenalarm.Frontend.Forms.WinPhone.Services
{
    public class LocaliationService : ILocalizationService
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
