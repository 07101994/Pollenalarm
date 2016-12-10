using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
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
            await App.Bootstrapper.PollenViewModel.SaveChangesAsync(App.Bootstrapper.PollenViewModel.CurrentPollen);
		}
    }
}