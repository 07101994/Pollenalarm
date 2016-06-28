using System;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.ViewModels;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class PlaceViewModel : AsyncViewModelBase
	{
		private Place _CurrentPlace;
		public Place CurrentPlace
		{
			get { return _CurrentPlace; }
			set { _CurrentPlace = value; RaisePropertyChanged(); }
		}

		public PlaceViewModel()
		{
		}
	}
}

