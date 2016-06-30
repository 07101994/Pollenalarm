using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class PollenViewModel : AsyncViewModelBase
    {
        private Pollen _CurrentPollen;
        public Pollen CurrentPollen
        {
            get { return _CurrentPollen; }
            set { _CurrentPollen = value; RaisePropertyChanged(); }
        }

        public PollenViewModel()
        {

        }
    }
}
