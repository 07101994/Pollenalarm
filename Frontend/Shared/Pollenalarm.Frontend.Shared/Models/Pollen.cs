using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class Pollen : IPollen, ISearchResult
    {
        #region Implementation of IPollen

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BloomStart { get; set; }
        public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageCredits { get; set; }

        #endregion

        public string ImageName { get { return $"{Name.Replace("ß", "ss").Replace("ä", "ae")}.png"; } }
        public bool IsSelected { get; set; }
    }
}
