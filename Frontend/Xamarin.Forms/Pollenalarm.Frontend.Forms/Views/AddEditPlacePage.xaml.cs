using Pollenalarm.Frontend.Forms.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class AddEditPlacePage : ContentPage
    {
        public AddEditPlacePage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.AddEditPlaceViewModel;

            App.Bootstrapper.AddEditPlaceViewModel.OnInvalidEntries += PlaceViewModel_OnInvalidEntries;
            App.Bootstrapper.AddEditPlaceViewModel.OnLocationFailed += PlaceViewModel_OnLocationFailed;

			if (App.Bootstrapper.PlaceViewModel.CurrentPlace != null)
			{
				// Edit existing place
				Title = "Edit";
				AddButton.Text = "Save";
			}
			else
			{
				// Add new place
				Title = "New Place";
				AddButton.Text = "Add";
				ToolbarItems.Remove(DeleteToolbarItem);
			}
        }

        protected override void OnAppearing()
        {
            // Unsubscribe
            App.Bootstrapper.AddEditPlaceViewModel.OnInvalidEntries -= PlaceViewModel_OnInvalidEntries;
            App.Bootstrapper.AddEditPlaceViewModel.OnLocationFailed -= PlaceViewModel_OnLocationFailed;
        }

        private void PlaceViewModel_OnInvalidEntries(object sender, EventArgs e)
        {
            DisplayAlert(Strings.InvalidEntriesTitle, Strings.InvalidEntriesMessage, Strings.OK);
        }

        private void PlaceViewModel_OnLocationFailed(object sender, EventArgs e)
        {
            DisplayAlert(Strings.GeoLocationFailedTitle, Strings.GeoLocationFailedMessage, Strings.OK);
        }
    }
}
