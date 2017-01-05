using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Info;

namespace Pollenalarm.Old.WinPhone.Helper
{
    public static class LowMemoryHelper
    {
        public static bool IsLowMemDevice { get; set; }

        static LowMemoryHelper()
        {
            try
            {
                Int64 result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
                if (result < 94371840L)
                    IsLowMemDevice = true;
                else
                    IsLowMemDevice = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Windows Phone OS update not installed, which indicates a 512-MB device. 
                IsLowMemDevice = false;
            }
        }
    }

}
