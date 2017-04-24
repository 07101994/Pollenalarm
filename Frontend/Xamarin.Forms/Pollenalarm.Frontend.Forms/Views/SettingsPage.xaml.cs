using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await App.Bootstrapper.SettingsViewModel.InitializeAsync();

            if (BindingContext == null)
                BindingContext = App.Bootstrapper.SettingsViewModel;
        }

        private async void Settings_Changed(object sender, EventArgs e)
        {
            await App.Bootstrapper.SettingsViewModel.SaveChangesAsnyc();
        }
    }
}
