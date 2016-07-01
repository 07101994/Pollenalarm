using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class PlacePage : TabbedPage
    {
        public PlacePage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.PlaceViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.Bootstrapper.PlaceViewModel.CurrentPlace == null)
                Navigation.PopAsync();
        }

        private void PollutionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Execute ViewModel command in code behind here, since it not possible to bind a Command to a ListView's ItemSelected event in Xamarin.Forms yet
            var pollution = e.SelectedItem as Pollution;
            if (pollution != null && pollution.Pollen != null)
            {
                ((ListView)sender).SelectedItem = null;
                App.Bootstrapper.PlaceViewModel.NavigateToPollenCommand.Execute(pollution.Pollen);
            }
        }
    }
}
