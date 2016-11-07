using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Forms.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Collections.Specialized;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class PlacePage : TabbedPage
    {
        private SettingsService _SettingsService;

        public PlacePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, Strings.Back);
            BindingContext = App.Bootstrapper.PlaceViewModel;

            _SettingsService = App.Bootstrapper.SettingsService;

            // Hide edit button if selected place is auto-generated current position
            if (App.Bootstrapper.PlaceViewModel.CurrentPlace.IsCurrentPosition)
                ToolbarItems.Remove(EditPlaceButton);
        }

        private async void PlacePage_CurrentPageChanged(object sender, EventArgs e)
        {
            // Animate Tab change on iOS as it is not implemented in Xamarin.Forms yet.
            if (Device.OS == TargetPlatform.iOS)
            {
                uint duration = 300;
                await Task.WhenAll(CurrentPage.TranslateTo(0, 1000, duration), CurrentPage.FadeTo(0, duration));
                await Task.WhenAll(CurrentPage.TranslateTo(0, 0, duration), CurrentPage.FadeTo(1, duration));
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Navigate back to MainPage, when CurrentPlace is null.
            // This can happen, after a place has been deleted, for example.
            if (App.Bootstrapper.PlaceViewModel.CurrentPlace == null)
            {
                await Navigation.PopAsync();
                return;
            }

            // Update pollen selections
            App.Bootstrapper.PlaceViewModel.Update();

            // Filter list by selected pollen when this is activated in the settings by code, because ListView Filters are not supported by Xamarin.Forms yet
            await _SettingsService.LoadSettingsAsync();
            if (_SettingsService.CurrentSettings.ShowSelectedPollenOnly)
            {
                ListToday.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday.Where(p => p.Pollen.IsSelected);
                ListTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow.Where(p => p.Pollen.IsSelected);
                ListAfterTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow.Where(p => p.Pollen.IsSelected);
            }
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
