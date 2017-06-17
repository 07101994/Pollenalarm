using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollenSelectionPage : ContentPage
    {
        public PollenSelectionPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
                BindingContext = App.Bootstrapper.PollenViewModel;

            //if (!App.Bootstrapper.PollenViewModel.IsLoaded)
            //await App.Bootstrapper.PollenViewModel.RefreshAsync();
        }

        protected override void OnDisappearing()
        {
            //await App.Bootstrapper.PollenViewModel.SaveChangesAsync();
            //base.OnDisappearing();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Unselect
            (sender as ListView).SelectedItem = null;
        }
    }
}
