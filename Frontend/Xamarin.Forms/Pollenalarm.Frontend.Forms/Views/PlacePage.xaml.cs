using System;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Forms.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class PlacePage : TabbedPage
	{
		public PlacePage()
		{
			InitializeComponent();
			NavigationPage.SetBackButtonTitle(this, Strings.Back);
			BindingContext = App.Bootstrapper.PlaceViewModel;
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
			await App.Bootstrapper.PlaceViewModel.RefreshAsync();

            // Filtering has performance problems currently.
            // This is why the BoolToTextColorConverter currently paints disabled items gray
            ListToday.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday;
            ListTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow;
            ListAfterTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow;

            #region Filter Attempts

            // Filter list by selected pollen by code, because ListView Filters are not supported by Xamarin.Forms yet
            //ListToday.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday.Where(p => p.Pollen.IsSelected);
            //ListTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow.Where(p => p.Pollen.IsSelected);
            //ListAfterTomorrow.ItemsSource = App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow.Where(p => p.Pollen.IsSelected);

            // Maybe the part below is more performant, as it does not filter 3 times? Test on a real device / profiler
            //var todayList = new ObservableCollection<Pollution>();
            //var tomorrowList = new ObservableCollection<Pollution>();
            //var afterTomorrowList = new ObservableCollection<Pollution>();
            //await Task.Factory.StartNew(() =>
            //{
            //	todayList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday);
            //	tomorrowList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionTomorrow);
            //	afterTomorrowList = new ObservableCollection<Pollution>(App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionAfterTomorrow);
            //	for (int i = 0; i < App.Bootstrapper.PlaceViewModel.CurrentPlace.PollutionToday.Count() - 1; i++)
            //	{
            //		if (!todayList[i].Pollen.IsSelected)
            //		{
            //			todayList.RemoveAt(i);
            //			tomorrowList.RemoveAt(i);
            //			afterTomorrowList.RemoveAt(i);
            //		}
            //	}
            //});

            //ListToday.ItemsSource = todayList;
            //ListTomorrow.ItemsSource = tomorrowList;
            //ListAfterTomorrow.ItemsSource = afterTomorrowList;

            #endregion
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
