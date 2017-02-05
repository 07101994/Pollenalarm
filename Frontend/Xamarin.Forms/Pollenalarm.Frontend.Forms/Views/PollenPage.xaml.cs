using System;
using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PollenPage : ContentPage
	{
		private PollenViewModel viewModel;

		public PollenPage()
		{
			InitializeComponent();
			BindingContext = viewModel = App.Bootstrapper.PollenViewModel;

			// Translate Bloom Time range
			BloomTime.Text = $"{viewModel.CurrentPollen.BloomStart.ToString("MMMM")} {Strings.Until} {viewModel.CurrentPollen.BloomEnd.ToString("MMMM")}";
		}

		private async void AllergySwitch_Toggled(object sender, EventArgs e)
		{
            AllergySwitch.IsEnabled = false;
			await App.Bootstrapper.PollenViewModel.SaveChangesAsync(App.Bootstrapper.PollenViewModel.CurrentPollen);
            AllergySwitch.IsEnabled = true;
        }
	}
}