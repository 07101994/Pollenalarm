using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms
{
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper;

        public App()
        {
            InitializeComponent();

            // Initialize Bootstrapper
            if (Bootstrapper == null)
                Bootstrapper = new Bootstrapper((NavigationPage)MainPage);

            // Create Navigation page
            var navigationPage = new NavigationPage(new MainPage());
            Device.OnPlatform(iOS: () => { navigationPage.BarTextColor = Color.White; });

            // Initialize NavigationService using the navigation page
            Bootstrapper.RegisterNavigationService(navigationPage);

            // Spin-up application
            if (MainPage == null)
                MainPage = navigationPage;
        }
    }
}
