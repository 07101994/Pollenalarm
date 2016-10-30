using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Forms.Services
{
    public interface ILocalizationService
    {
        CultureInfo GetCurrentCultureInfo();
    }
}
