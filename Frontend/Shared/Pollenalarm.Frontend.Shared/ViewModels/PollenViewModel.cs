using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class PollenViewModel : AsyncViewModelBase
    {
		private PollenService _PollenService;
		private SettingsService _SettingsService;

		private ObservableCollection<Pollen> _Pollen;
		public ObservableCollection<Pollen> Pollen
		{
			get { return _Pollen; }
			set { _Pollen = value; RaisePropertyChanged(); }
		}

        private Pollen _CurrentPollen;
        public Pollen CurrentPollen
        {
            get { return _CurrentPollen; }
            set { _CurrentPollen = value; RaisePropertyChanged(); }
        }

		public PollenViewModel(PollenService pollenService, SettingsService settingsService)
        {
			_PollenService = pollenService;
			_SettingsService = settingsService;
        }
    }
}
