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
			Title = (App.Bootstrapper.PlaceViewModel.CurrentPlace != null) ? "Edit" : "New Place";
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
    }
}
