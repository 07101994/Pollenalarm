using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pollenalarm.Frontend.Forms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pollenalarm.Frontend.Forms.Droid.Services.LocalizationService))]
namespace Pollenalarm.Frontend.Forms.Droid.Services
{
    public class LocalizationService : ILocalizationService
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-"); // turns pt_BR into pt-BR
            return new System.Globalization.CultureInfo(netLanguage);
        }
}
}