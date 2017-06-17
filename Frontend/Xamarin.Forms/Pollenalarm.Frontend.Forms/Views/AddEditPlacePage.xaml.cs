using System;
using Pollenalarm.Frontend.Forms.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GalaSoft.MvvmLight.Ioc;
using Pollenalarm.Frontend.Shared.Services;
using Pollenalarm.Frontend.Shared.ViewModels;

namespace Pollenalarm.Frontend.Forms.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPlacePage : ContentPage
    {
        private AddEditPlaceViewModel viewModel;

        public AddEditPlacePage()
        {
            InitializeComponent();
            BindingContext = viewModel = App.Bootstrapper.AddEditPlaceViewModel;

            var placeService = SimpleIoc.Default.GetInstance<PlaceService>();

            if (placeService.CurrentPlace != null)
            {
                // Edit existing place
                Title = "Edit";
                AddButton.Text = "Save";
            }
            else
            {
                // Add new place
                Title = "New Place";
                AddButton.Text = "Add";
                ToolbarItems.Remove(DeleteToolbarItem);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.Refresh();
        }
    }
}
