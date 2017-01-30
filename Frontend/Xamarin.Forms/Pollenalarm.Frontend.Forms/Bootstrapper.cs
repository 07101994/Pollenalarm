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
using Pollenalarm.Core;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Forms
{
    public class Bootstrapper
    {
        public Bootstrapper(NavigationPage navigationPage = null)
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Reset();

            // Services
            SimpleIoc.Default.Register<IFileSystemService, FileSystemService>();
			SimpleIoc.Default.Register<IGeoLoactionService, GeoLocationService>();
			SimpleIoc.Default.Register<IHttpService, HttpService>();
            SimpleIoc.Default.Register<ILocalizationService, LocalizationService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<PollenService>();
            SimpleIoc.Default.Register<GoogleMapsService>();
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<PlaceService>();

            // ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<PlaceViewModel>();
			SimpleIoc.Default.Register<PollenViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AddEditPlaceViewModel>();
        }

        public MainViewModel MainViewModel { get { return SimpleIoc.Default.GetInstance<MainViewModel>(); } }
		public PlaceViewModel PlaceViewModel { get { return SimpleIoc.Default.GetInstance<PlaceViewModel>(); }}
		public PollenViewModel PollenViewModel { get { return SimpleIoc.Default.GetInstance<PollenViewModel>(); } }
		public SearchViewModel SearchViewModel { get { return SimpleIoc.Default.GetInstance<SearchViewModel>(); } }
        public SettingsViewModel SettingsViewModel { get { return SimpleIoc.Default.GetInstance<SettingsViewModel>(); } }
        public AddEditPlaceViewModel AddEditPlaceViewModel { get { return SimpleIoc.Default.GetInstance<AddEditPlaceViewModel>(); } }

        public SettingsService SettingsService { get { return SimpleIoc.Default.GetInstance<SettingsService>(); } }

        public void RegisterNavigationService(NavigationPage navigationPage)
        {
            var navigationService = new NavigationService(navigationPage);

            navigationService.Configure(ViewNames.Main, typeof(MainPage));
            navigationService.Configure(ViewNames.Place, typeof(PlacePage));
            navigationService.Configure(ViewNames.Pollen, typeof(PollenPage));
            navigationService.Configure(ViewNames.Settings, typeof(SettingsPage));
            navigationService.Configure(ViewNames.AddEditPlace, typeof(AddEditPlacePage));
            navigationService.Configure(ViewNames.About, typeof(AboutPage));
            navigationService.Configure(ViewNames.PollenSelection, typeof(PollenSelectionPage));
            navigationService.Configure(ViewNames.Search, typeof(SearchPage));

            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
                SimpleIoc.Default.Unregister<INavigationService>();

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
        }
    }
}