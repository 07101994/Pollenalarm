using Pollenalarm.Frontend.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms
{
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper;

        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new MainPage());
            Bootstrapper = new Bootstrapper(navigationPage);
            MainPage = navigationPage;
        }
    }
}
