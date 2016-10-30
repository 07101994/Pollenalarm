using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            await App.Bootstrapper.SettingsViewModel.InitializeAsync();

			if (BindingContext == null)
				BindingContext = App.Bootstrapper.SettingsViewModel;
		}

        private async void Settings_Changed(object sender, EventArgs e)
        {
            await App.Bootstrapper.SettingsViewModel.SaveChangesAsnyc();
        }
    }
}
