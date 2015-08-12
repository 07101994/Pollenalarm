using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pollenalarm.Backend.Controllers.Base
{
    public class PollenalarmApiControllerBase : ApiController
    {
        protected DataContext DataContext;

        protected PollenalarmApiControllerBase()
        {
            DataContext = new DataContext(ConfigurationManager.ConnectionStrings["AzureDatabaseConnection"].ConnectionString);
        }
    }
}
