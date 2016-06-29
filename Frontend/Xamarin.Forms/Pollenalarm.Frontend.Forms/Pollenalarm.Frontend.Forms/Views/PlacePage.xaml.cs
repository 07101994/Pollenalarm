using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class PlacePage : ContentPage
    {
        public PlacePage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.PlaceViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Title = App.Bootstrapper.PlaceViewModel.CurrentPlace.Name;
        }
    }
}
