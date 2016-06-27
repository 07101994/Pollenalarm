using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.MainViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!App.Bootstrapper.MainViewModel.IsLoaded)
                await App.Bootstrapper.MainViewModel.RefreshAsync();

        }

        private async void Settings_Activated(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}
