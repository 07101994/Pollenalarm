using System;
using Pollenalarm.Frontend.Forms.Resources;
using Pollenalarm.Frontend.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollenPage : ContentPage
    {
        private PollenViewModel viewModel;

        public PollenPage()
        {
            InitializeComponent();
            BindingContext = viewModel = App.Bootstrapper.PollenViewModel;
            viewModel.Refresh();
        }

        private async void AllergySwitch_Toggled(object sender, EventArgs e)
        {
            await viewModel.SaveChangesAsync();
        }
    }
}