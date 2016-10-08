using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services.Base;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Pollenalarm.Backend.Services
{
    public class PlaceService : ServiceBase
    {
        private Table<CityEntity> cityTable;

        public PlaceService()
        {
            cityTable = DataContext.GetTable<CityEntity>();
        }
    }
}