using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Shared.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Pollenalarm.Frontend.Shared.ViewModels;

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
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    // Hide Add, as we use a Floating Action Button
                    ToolbarItems.Remove(AddItem);
                    // Hide Refresh, as we use pull-to-refresh
                    ToolbarItems.Remove(RefreshItem);
                    break;

                case Device.iOS:
                    // Hide Refresh, as we use pull-to-refresh
                    ToolbarItems.Remove(RefreshItem);
                    break;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
                BindingContext = App.Bootstrapper.MainViewModel;

            await App.Bootstrapper.MainViewModel.RefreshAsync();

            // Hide No-Places-Warning, because Binding does not work
            //lblNoPlacesWarning.IsVisible = !App.Bootstrapper.MainViewModel.Places.Any();
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var selectedPlace = e.Item as PlaceRowViewModel;
            if (selectedPlace != null)
            {
                App.Bootstrapper.MainViewModel.NavigateToPlaceCommand.Execute(selectedPlace.Id);
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}