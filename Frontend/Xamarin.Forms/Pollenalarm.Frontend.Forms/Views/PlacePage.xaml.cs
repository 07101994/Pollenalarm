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
using System.Collections.ObjectModel;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class PlacePage : TabbedPage
    {
        public PlacePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, Strings.Back);
            BindingContext = App.Bootstrapper.PlaceViewModel;

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

            // Filter list by selected pollen by code, because ListView Filters are not supported by Xamarin.Forms yet
            ListToday.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday.Where(p => p.Pollen.IsSelected);
            ListTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow.Where(p => p.Pollen.IsSelected);
            ListAfterTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow.Where(p => p.Pollen.IsSelected);

            // Maybe the part below is more performant, as it does not filter 3 times? Test on a real device / profiler
            //var todayList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday);
            //var tomorrowList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow);
            //var afterTomorrowList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow);
            //for (int i = 0; i < App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday.Count() - 1; i++)
            //{
            //    if (!todayList[i].Pollen.IsSelected)
            //    {
            //        todayList.RemoveAt(i);
            //        tomorrowList.RemoveAt(i);
            //        afterTomorrowList.RemoveAt(i);
            //    }
            //}
            //ListToday.ItemsSource = todayList;
            //ListTomorrow.ItemsSource = tomorrowList;
            //ListAfterTomorrow.ItemsSource = afterTomorrowList;
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
