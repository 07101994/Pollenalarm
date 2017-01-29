using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.SearchViewModel;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Pollen)
            {
                var pollen = e.SelectedItem as Pollen;
                if (pollen != null)
                {
                    ((ListView)sender).SelectedItem = null;
                    App.Bootstrapper.SearchViewModel.NavigateToPollenCommand.Execute(pollen);
                }

                return;
            }

            if (e.SelectedItem is Place)
            {
                var place = e.SelectedItem as Place;
                if (place != null)
                {
                    ((ListView)sender).SelectedItem = null;
                    App.Bootstrapper.SearchViewModel.NavigateToPlaceCommand.Execute(place);
                }

                return;
            }
        }
    }
}
