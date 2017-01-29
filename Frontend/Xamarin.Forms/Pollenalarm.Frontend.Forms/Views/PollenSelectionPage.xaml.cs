using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Forms.Resources;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollenSelectionPage : ContentPage
	{
		public PollenSelectionPage()
		{
			InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
                BindingContext = App.Bootstrapper.PollenViewModel;

            if (!App.Bootstrapper.PollenViewModel.IsLoaded)
                await App.Bootstrapper.PollenViewModel.RefreshAsync();
        }

        protected override async void OnDisappearing()
        {
            await App.Bootstrapper.PollenViewModel.SaveChangesAsync();
            base.OnDisappearing();
        }
    }
}
