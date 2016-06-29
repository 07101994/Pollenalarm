using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Pollenalarm.Frontend.Forms.Services;
using Pollenalarm.Frontend.Shared.Services;
using Pollenalarm.Frontend.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Frontend.Shared;
using Xamarin.Forms;
using Pollenalarm.Frontend.Forms.Views;

namespace Pollenalarm.Frontend.Forms
{
    public class Bootstrapper
    {
        public Bootstrapper(NavigationPage navigationPage)
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Services
            SimpleIoc.Default.Register<INavigationService>(() => CreateNavigationService(navigationPage));
            SimpleIoc.Default.Register<PollenService>();

			// ViewModels
			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<PlaceViewModel>();
        }

        public MainViewModel MainViewModel { get { return SimpleIoc.Default.GetInstance<MainViewModel>(); } }
		public PlaceViewModel PlaceViewModel { get { return SimpleIoc.Default.GetInstance<PlaceViewModel>(); }}

        private INavigationService CreateNavigationService(NavigationPage navigationPage)
        {
            var navigationService = new NavigationService(navigationPage);
            navigationService.Configure("Main", typeof(MainPage));
            navigationService.Configure("Settings", typeof(SettingsPage));
            navigationService.Configure("Place", typeof(PlacePage));
            navigationService.Configure("AddEditPlace", typeof(AddEditPlacePage));
            return navigationService;
        }
    }
}