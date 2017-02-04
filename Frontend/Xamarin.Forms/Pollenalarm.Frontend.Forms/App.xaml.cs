using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
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
			Device.OnPlatform(iOS: () => { navigationPage.BarTextColor = Color.White; });

			// Initialize NavigationService using the navigation page
			Bootstrapper.RegisterNavigationService(navigationPage);

			// Spin everything up
			MainPage = navigationPage;
			MainPage.On<Xamarin.Forms.PlatformConfiguration.Windows>().SetToolbarPlacement(ToolbarPlacement.Bottom);
		}
	}
}
