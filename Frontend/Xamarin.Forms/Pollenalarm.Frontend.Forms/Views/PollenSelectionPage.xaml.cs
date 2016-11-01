using Pollenalarm.Frontend.Forms.Resources;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms
{
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
            base.OnDisappearing();
            await App.Bootstrapper.PollenViewModel.SaveChangesAsync();
        }
    }
}
