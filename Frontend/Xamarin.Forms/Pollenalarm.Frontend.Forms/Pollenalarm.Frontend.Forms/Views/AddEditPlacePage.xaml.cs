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
			BindingContext = App.Bootstrapper.PlaceViewModel;
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

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
    }
}
