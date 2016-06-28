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

namespace Pollenalarm.Frontend.Forms
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Services
            SimpleIoc.Default.Register<PollenService>();

			// ViewModels
			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<PlaceViewModel>();
        }

		public MainViewModel MainViewModel { get { return SimpleIoc.Default.GetInstance<MainViewModel>(); } }
		public PlaceViewModel PlaceViewModel { get { return SimpleIoc.Default.GetInstance<PlaceViewModel>(); }}
    }
}