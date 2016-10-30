using Pollenalarm.Frontend.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Pollenalarm.Frontend.Forms
{
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper;

        public App()
        {
            InitializeComponent();

            if (MainPage == null)
                MainPage = new NavigationPage(new MainPage());

            if (Bootstrapper == null)
                Bootstrapper = new Bootstrapper((NavigationPage)MainPage);

        }
    }
}
