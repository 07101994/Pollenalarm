using System;
using Pollenalarm.Frontend.Forms.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPlacePage : ContentPage
    {
        public AddEditPlacePage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.AddEditPlaceViewModel;

			if (App.Bootstrapper.AddEditPlaceViewModel.CurrentPlace != null)
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
