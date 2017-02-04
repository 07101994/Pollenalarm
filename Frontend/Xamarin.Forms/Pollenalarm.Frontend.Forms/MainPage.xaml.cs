using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Forms.Resources;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			NavigationPage.SetBackButtonTitle(this, Strings.Back);

			// Platform specific adjustments
			Device.OnPlatform(Android: () =>
			{
				// Hide Add, as we use a Floating Action Button
				ToolbarItems.Remove(AddItem);
				// Hide Refresh, as we use pull-to-refresh
				ToolbarItems.Remove(RefreshItem);
			});
			Device.OnPlatform(iOS: () =>
			{
				// Hide Refresh, as we use pull-to-refresh
				ToolbarItems.Remove(RefreshItem);

			});


			var listView = new ListView();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext == null)
				BindingContext = App.Bootstrapper.MainViewModel;

			if (!App.Bootstrapper.MainViewModel.IsLoaded)
			{
				await App.Bootstrapper.MainViewModel.RefreshAsync();
			}

			// Hide No-Places-Warning, because Binding does not work
			//lblNoPlacesWarning.IsVisible = !App.Bootstrapper.MainViewModel.Places.Any();
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