using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class Pollution : IPollution
    {
        #region Implementation of IPollution

        public string Id { get; set; }
        public string Zip { get; set; }
        public string PollenId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Updated { get; set; }
        public int Intensity { get; set; }

        #endregion

        /// <summary>
        /// Represents the full Pollen object inside the Frontend applications later
        /// but will not get represented in the database. So the frontend will need
        /// to fill this property based on the <see cref="PollenId"/>.
        /// </summary>
        public Pollen Pollen { get; set; }
    }
}
