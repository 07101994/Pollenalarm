using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Frontend.Shared.Misc;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class SettingsViewModel : AsyncViewModelBase
	{
		private INavigationService _NavigationService;

		private RelayCommand _NavigateToAboutCommand;
		public RelayCommand NavigateToAboutCommand
		{
			get
			{
				return _NavigateToAboutCommand ?? (_NavigateToAboutCommand = new RelayCommand(() =>
				{
					_NavigationService.NavigateTo(ViewNames.About);
				}));
			}
		}

		public SettingsViewModel(INavigationService navigationService)
		{
			_NavigationService = navigationService;
		}
	}
}
