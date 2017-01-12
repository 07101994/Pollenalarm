using Pollenalarm.Frontend.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.SearchViewModel;
        }
    }
}
