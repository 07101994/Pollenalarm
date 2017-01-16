using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.TemplateSelectors
{
    public class SearchResultDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PollenTemplate { get; set; }
        public DataTemplate PlaceTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Pollen)
                return PollenTemplate;
            if (item is Place)
                return PlaceTemplate;

            //return SelectTemplate(item, container);
            return null;
        }
    }
}
