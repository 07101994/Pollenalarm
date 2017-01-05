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
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Marketplace;
using System.Device.Location;
using Pollenalarm.Old.WinPhone.Helper;
using Pollenalarm.Old.WinPhone.ViewModels;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            // Update TimeSpan From
            if (App.Settings.Contains("UpdateSpanFrom"))
            {
                tpUpdateFrom.Value = ((DateTime)App.Settings["UpdateSpanFrom"]);
            }
            else
            {
                tpUpdateFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 30, 0);
            }

            // Update TimeSpan To
            if (App.Settings.Contains("UpdateSpanTo"))
            {
                tpUpdateTo.Value = ((DateTime)App.Settings["UpdateSpanTo"]);
            }
            else
            {
                tpUpdateTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 0);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Begin();

            // -----------------------------------
            // Load Settings
            // -----------------------------------
            CheckSettings();

            // Location Services
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.Start();

            if (watcher.Permission != GeoPositionPermission.Denied)
            {
                if (App.CheckSettings("LocationServicesAllowed"))
                {
                    tgsLocation.IsChecked = true;
                }
                else
                {
                    tgsLocation.IsChecked = false;
                }
            }
            else
            {
                tgsLocation.IsChecked = false;
                tgsLocation.IsEnabled = false;
                spnLocationDenied.Visibility = Visibility.Visible;
            }


            // Backgrond Task
            if (App.Settings.Contains("BackgroundTask"))
            {
                if (((bool)App.Settings["BackgroundTask"]) == true)
                {
                    tgsBackgroundTask.IsChecked = true;
                }
                else
                {
                    tgsBackgroundTask.IsChecked = false;
                }
            }
            else
            {
                tgsBackgroundTask.IsChecked = true;
            }

            // Update Always
            if (App.Settings.Contains("UpdateAlways"))
            {
                if (((bool)App.Settings["UpdateAlways"]) == true)
                {
                    cbUpdateAlways.IsChecked = true;
                }
                else
                {
                    cbUpdateAlways.IsChecked = false;
                }
            }
            else
            {
                cbUpdateAlways.IsChecked = true;
            }

            // Show all pollen
            if (lpShowPollen != null)
            {
                if (App.Settings.Contains("ShowAllPollen"))
                {
                    if (((bool)App.Settings["ShowAllPollen"]) == true)
                        lpShowPollen.SelectedIndex = 0;
                    else
                        lpShowPollen.SelectedIndex = 1;
                }
                else
                {
                    lpShowPollen.SelectedIndex = 0;
                }
            }

            // Background Image
            if (tgsBackgroundImage != null)
            {
                if (App.Settings.Contains("BackgroundImage"))
                {
                    if (((bool)App.Settings["BackgroundImage"]) == true)
                        tgsBackgroundImage.IsChecked = true;
                    else
                        tgsBackgroundImage.IsChecked = false;
                }
                else
                {
                    tgsBackgroundImage.IsChecked = true;
                }
            }

            // Shake To Refresh
            if (tgsShakeToRefresh != null)
            {
                if (!Accelerometer.IsSupported)
                {
                    tgsShakeToRefresh.IsChecked = false;
                    tgsShakeToRefresh.IsEnabled = false;
                }
                else
                {
                    if (App.Settings.Contains("ShakeToRefresh"))
                    {
                        if (((bool)App.Settings["ShakeToRefresh"]) == true)
                            tgsShakeToRefresh.IsChecked = true;
                        else
                            tgsShakeToRefresh.IsChecked = false;
                    }
                    else
                    {
                        tgsShakeToRefresh.IsChecked = true;
                    } 
                }
            }

            // Check Place on map

            if (App.Settings.Contains("CheckPlace"))
            {
                if (((bool)App.Settings["CheckPlace"]) == true)
                    tgsCheckPlace.IsChecked = true;
                else
                    tgsCheckPlace.IsChecked = false;
            }
            else
            {
                tgsCheckPlace.IsChecked = false;
            } 

            // Update Frequence
            if (App.LoadFromSettings("UpdateFrequence", null) != null)
            {
                lpkUpdateFrequence.SelectedIndex = (int)App.LoadFromSettings("UpdateFrequence", null);
            }
            else
            {
                lpkUpdateFrequence.SelectedIndex = 2;
            }

            // Check Low MB Device
            if (LowMemoryHelper.IsLowMemDevice)
            {
                PivotBackgroundTask.IsEnabled = false;
                tbxLowMBWarning.Visibility = Visibility.Visible;
                tgsBackgroundTask.IsChecked = false;
            }


            // Check Buy Button
            LicenseInformation license = new LicenseInformation();
            if (!license.IsTrial() && App.hideTrialsOnFullVersion == true)
            {
                spnRemoveAds.Visibility = Visibility.Collapsed;
            }

            // Check Time Difference
            CheckTime();
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

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (App.CheckSettings("LocationServicesAllowed"))
            {
                MainViewModel.Current.AddCurrentPosition();
            }
            else
            {
                MainViewModel.Current.RemoveCurrentPosition();
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            spnLocationWarning.Visibility = Visibility.Collapsed;
            tgsLocation.Content = "Erlaubt";
            App.SaveToSettings("LocationServicesAllowed", true);
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            spnLocationWarning.Visibility = Visibility.Visible;
            tgsLocation.Content = "Verweigert";
            App.SaveToSettings("LocationServicesAllowed", false);
        }

        private void cbUpdateAlways_Checked(object sender, RoutedEventArgs e)
        {
            if (tpUpdateFrom != null && tpUpdateTo != null)
            {
                App.SaveToSettings("UpdateAlways", true);
                tpUpdateFrom.IsEnabled = false;
                tpUpdateTo.IsEnabled = false;
                CheckTime();
            }
        }

        private void cbUpdateAlways_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tpUpdateFrom != null && tpUpdateTo != null)
            {
                App.SaveToSettings("UpdateAlways", false);
                tpUpdateFrom.IsEnabled = true;
                tpUpdateTo.IsEnabled = true;
                CheckTime();
            }
        }

        private void tpUpdateFrom_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (tpUpdateFrom != null)
            {
                App.SaveToSettings("UpdateSpanFrom", ((DateTime)tpUpdateFrom.Value));
                CheckTime();
            }
        }

        private void tpUpdateTo_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (tpUpdateTo != null)
            {
                App.SaveToSettings("UpdateSpanTo", ((DateTime)tpUpdateTo.Value));
                CheckTime();
            }
        }

        /// <summary>
        /// Checks whether the timepsan between the update times is 30 minutes and shows warning if not
        /// </summary>
        private void CheckTime()
        {
            TimeSpan difference;

            if (((DateTime)tpUpdateFrom.Value).CompareTo((DateTime)tpUpdateTo.Value) > 0)
            {
                difference = ((DateTime)tpUpdateTo.Value).AddDays(1) - (DateTime)tpUpdateFrom.Value;
            }
            else
            {
                difference = (DateTime)tpUpdateTo.Value - (DateTime)tpUpdateFrom.Value;
            }

            //MessageBox.Show("Difference: " + tpUpdateFrom.Value + " -- " + tpUpdateTo.Value + ": " + difference.TotalMinutes);

            if (difference.TotalMinutes < 45 && cbUpdateAlways.IsChecked == false)
            {
                spnTimeWarning.Visibility = Visibility.Visible;
            }
            else
            {
                spnTimeWarning.Visibility = Visibility.Collapsed;
            }
        }

        private void lpShowPollen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (lpShowPollen != null)
            {
                switch (lpShowPollen.SelectedIndex)
                {
                    case 0:
                        App.SaveToSettings("ShowAllPollen", true);
                        App.ShowAllPollen = true;
                        break;
                    case 1:
                        App.SaveToSettings("ShowAllPollen", false);
                        App.ShowAllPollen = false;
                        break;
                }
            }
        }

        private void lpkUpdateFrequence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lpkUpdateFrequence != null)
            {
                App.SaveToSettings("UpdateFrequence", lpkUpdateFrequence.SelectedIndex);
            }
        }

        private void tgsBackgroundTask_Checked(object sender, RoutedEventArgs e)
        {
            if (tgsBackgroundTask != null)
            {
                tgsBackgroundTask.Content = "Aktiviert";
                App.SaveToSettings("BackgroundTask", true);
                MainPage.StartPeriodicAgent();
            }
        }

        private void tgsBackgroundTask_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tgsBackgroundTask != null)
            {
                tgsBackgroundTask.Content = "Deaktiviert";
                App.SaveToSettings("BackgroundTask", false);
                MainPage.StopPeriodicAgent();
            }
        }

        private void tgsBackgroundImage_Checked(object sender, RoutedEventArgs e)
        {
            if (tgsBackgroundImage != null)
            {
                tgsBackgroundImage.Content = "Ein";
                App.SaveToSettings("BackgroundImage", true);
                BackgroundImage.Opacity = 1;
            }
        }

        private void tgsBackgroundImage_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tgsBackgroundImage != null)
            {
                tgsBackgroundImage.Content = "Aus";
                App.SaveToSettings("BackgroundImage", false);
                BackgroundImage.Opacity = 0;
            }
        }

        private void tgsShakeToRefresh_Checked(object sender, RoutedEventArgs e)
        {
            if (tgsShakeToRefresh != null)
            {
                tgsShakeToRefresh.Content = "Ein";
                App.SaveToSettings("ShakeToRefresh", true);
            }
        }

        private void tgsShakeToRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tgsShakeToRefresh != null)
            {
                tgsShakeToRefresh.Content = "Aus";
                App.SaveToSettings("ShakeToRefresh", false);
            }
        }

        private void tgsCheckPlace_Checked(object sender, RoutedEventArgs e)
        {
            if (tgsCheckPlace != null)
            {
                tgsCheckPlace.Content = "Ein";
                App.SaveToSettings("CheckPlace", true);
            }
        }

        private void tgsCheckPlace_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tgsCheckPlace != null)
            {
                tgsCheckPlace.Content = "Aus";
                App.SaveToSettings("CheckPlace", false);
            }
        }

        private void btnRemoveAds_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Diese Version von Pollenalarm finanziert sich durch Werbeeinnahmen und kann somit kostenlos zur Verfügung gestellt werden. Um die Werbebanner dauerhaft zu entfernen kaufen Sie bitte die Vollversion. Wählen Sie \"Ok\", um zur Kaufseite zu gelangen!", "Werbebanner entfernen", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                MarketplaceDetailTask buyTheApp = new MarketplaceDetailTask();
                buyTheApp.Show();
            }
        }

        
    }
}