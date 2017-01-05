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
using Pollenalarm.Old.WinPhone.ViewModels;
using Pollenalarm.Old.WinPhone.Models;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class PollenSelection : PhoneApplicationPage
    {
        public PollenSelection()
        {
            InitializeComponent();            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            AnimationStart.Begin();
            CheckSettings();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            AnimationEnd.Begin();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Perform Offline Update for all Places to perform changes
            MainViewModel.Current.OfflineUpdateAllPlaces();         

            // Save Changes to Isolated Storage
            App.SaveToSettings("AllPollen", MainViewModel.Current.AllPollen);
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

        private void appBarCheckAll_Click(object sender, EventArgs e)
        {
            foreach (Pollen pollen in MainViewModel.Current.AllPollen)
            {
                pollen.IsSelected = true;
            }
        }

        private void appBarUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (Pollen pollen in MainViewModel.Current.AllPollen)
            {
                pollen.IsSelected = false;
            }
        }
    }
}