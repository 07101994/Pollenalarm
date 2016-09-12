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
using Pollenalarm.Frontend.Shared.Misc;

namespace Pollenalarm.Frontend.Forms
{
    public class Bootstrapper
    {
        public Bootstrapper(NavigationPage navigationPage)
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Services
            SimpleIoc.Default.Register<INavigationService>(() => CreateNavigationService(navigationPage));
            SimpleIoc.Default.Register<IFileSystemService, FileSystemService>();
            SimpleIoc.Default.Register<PollenService>();
            SimpleIoc.Default.Register<IGeoLoactionService, GeoLocationService>();

			// ViewModels
			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<PlaceViewModel>();
			SimpleIoc.Default.Register<PollenViewModel>();
        }

        public MainViewModel MainViewModel { get { return SimpleIoc.Default.GetInstance<MainViewModel>(); } }
		public PlaceViewModel PlaceViewModel { get { return SimpleIoc.Default.GetInstance<PlaceViewModel>(); }}
		public PollenViewModel PollenViewModel { get { return SimpleIoc.Default.GetInstance<PollenViewModel>(); } }

        private INavigationService CreateNavigationService(NavigationPage navigationPage)
        {
            var navigationService = new NavigationService(navigationPage);
            navigationService.Configure(ViewNames.Main, typeof(MainPage));
            navigationService.Configure(ViewNames.Place, typeof(PlacePage));
            navigationService.Configure(ViewNames.Pollen, typeof(PollenPage));
            navigationService.Configure(ViewNames.Settings, typeof(SettingsPage));
            navigationService.Configure(ViewNames.AddEditPlace, typeof(AddEditPlacePage));
            navigationService.Configure(ViewNames.About, typeof(AboutPage));
            return navigationService;
        }
    }
}