using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Views
{
    public partial class PollenPage : ContentPage
    {
        public PollenPage()
        {
            InitializeComponent();
            BindingContext = App.Bootstrapper.PollenViewModel;
            //PollenImage.Source = ImageSource.FromFile(App.Bootstrapper.PollenViewModel.CurrentPollen.ImageName);
        }

		private async void AllergySwitch_Toggled(object sender, EventArgs e)
		{
            await App.Bootstrapper.PollenViewModel.SaveChangesAsync();
		}
    }
}
