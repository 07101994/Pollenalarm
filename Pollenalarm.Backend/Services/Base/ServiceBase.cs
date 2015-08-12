using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Pollenalarm.Backend.Services.Base
{
    public abstract class ServiceBase
    {
        protected DataContext DataContext;

        protected ServiceBase()
        {
            DataContext = new DataContext(ConfigurationManager.ConnectionStrings["AzureDatabaseConnection"].ConnectionString);
        }
    }
}