using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Pollenalarm.Old.WinPhone.ViewModels;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class PollenDetails : PhoneApplicationPage
    {
        public PollenDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            imgPollenImage.Source = new BitmapImage(new Uri("/Assets/Pollen/" + App.AsciName(MainViewModel.Current.CurrentPollen.Name) + ".png", UriKind.Relative));
            CheckSettings();

            AnimationStart.Begin();
        }

        private void CheckSettings()
        {
            // Background Image
            if (App.Settings.Contains("BackgroundImage"))
            {
                if (((bool)App.Settings["BackgroundImage"]) == true)
                    BackgroundImage.Opacity = 1;
                else
                    BackgroundImage.Opacity = 0;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            AnimationEnd.Begin();
        }
    }
}