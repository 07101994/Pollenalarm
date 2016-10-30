using Pollenalarm.Frontend.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// [assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Pollenalarm.Frontend.Forms
{
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper;

        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new MainPage());
            navigationPage.BarTextColor = Color.White;

            if (MainPage == null)
                MainPage = navigationPage;

            if (Bootstrapper == null)
                Bootstrapper = new Bootstrapper((NavigationPage)MainPage);

        }
    }
}
