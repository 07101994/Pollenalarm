using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using Pollenalarm.Frontend.Forms.Abstractions;

namespace Pollenalarm.Frontend.Forms
{
    //[XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper;

        public App()
        {
            InitializeComponent();

            // Initialize Bootstrapper
            Bootstrapper = new Bootstrapper();

            // Create Navigation page
            var navigationPage = new NavigationPage(new MainPage());

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                navigationPage.BarTextColor = Color.White;

            // Initialize NavigationService using the navigation page
            Bootstrapper.RegisterNavigationService(navigationPage);

            // Spin everything up
            MainPage = navigationPage;
            MainPage.On<Xamarin.Forms.PlatformConfiguration.Windows>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Only start Visual Studio Mobile Center when running in a real-world scenario
            var environment = DependencyService.Get<IEnvironmentService>();
            if (environment.IsRunningInRealWorld())
            {
                MobileCenter.Start(
                    "ios=85f19b77-10f3-464a-802d-4c8d9b0eac04;" +
                    "android=6be5a535-4b44-4c69-b4ca-73ff82dd96c9;" +
                    "uwp=f435af68-e540-4688-845f-435e0a1079f3;",
                    typeof(Analytics),
                    typeof(Crashes));
            }
        }
    }
}
