using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
                BindingContext = App.Bootstrapper.MainViewModel;

            if (!App.Bootstrapper.MainViewModel.IsLoaded)
                await App.Bootstrapper.MainViewModel.RefreshAsync();

            Device.OnPlatform(Android: () => { ToolbarItems.Remove(AddItem); });
        }

        private void PlacesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Execute ViewModel command in code behind here, since it not possible to bind a Command to a ListView's ItemSelected event in Xamarin.Forms yet
            var selectedPlace = e.SelectedItem as Place;
            if (selectedPlace != null)
            {
                ((ListView)sender).SelectedItem = null;
                App.Bootstrapper.MainViewModel.NavigateToPlaceCommand.Execute(selectedPlace);
            }
        }
    }
}