using System;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class PlaceViewModel : AsyncViewModelBase
	{
        private INavigationService _NavigationService;

		private Place _CurrentPlace;
		public Place CurrentPlace
		{
			get { return _CurrentPlace; }
			set { _CurrentPlace = value; RaisePropertyChanged(); }
        }

        #region Add / Edit fields

        private string _PlaceName;
        public string PlaceName
        {
            get { return _PlaceName; }
            set { _PlaceName = value; RaisePropertyChanged(); }
        }

        private string _PlaceZip;
        public string PlaceZip
        {
            get { return _PlaceZip; }
            set { _PlaceZip = value; RaisePropertyChanged(); }
        }


        #endregion

        private RelayCommand _AddEditPlaceCommand;
        public RelayCommand AddEditPlaceCommand
        {
            get
            {
                return _AddEditPlaceCommand ?? (_AddEditPlaceCommand = new RelayCommand(() =>
                {
                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();

                    if (_CurrentPlace != null)
                    {
                        var existingPlace = mainViewModel.Places.FirstOrDefault(x => x.Id == _CurrentPlace.Id);
                        if (existingPlace != null)
                        {
                            existingPlace.Name = _PlaceName;
                            existingPlace.Zip = _PlaceZip;
                        }
                    }
                    else
                    {
                        _CurrentPlace = new Place();
                        _CurrentPlace.Name = _PlaceName;
                        _CurrentPlace.Zip = _PlaceZip;
                        mainViewModel.Places.Add(_CurrentPlace);
                    }

                    _CurrentPlace = null;
                    _PlaceName = string.Empty;
                    _PlaceZip = string.Empty;
                    _NavigationService.GoBack();
                }));
            }
        }

        public PlaceViewModel(INavigationService navigationService)
		{
            _NavigationService = navigationService;
		}
	}
}

