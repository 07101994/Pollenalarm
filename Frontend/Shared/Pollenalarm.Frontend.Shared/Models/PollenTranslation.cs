using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class PollenTranslation : IPollenTranslation
    {
        #region Implementation of IPollenTranslation

        public string Id { get; set; }
        public string PollenId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #endregion
    }
}
