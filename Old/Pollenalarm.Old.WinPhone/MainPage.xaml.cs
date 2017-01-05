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
using Microsoft.Phone.Shell;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Marketplace;
using Microsoft.Devices.Sensors;
using System.Windows.Threading;
//using Microsoft.Xna.Framework;
//using ShakeGestures;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;
using Pollenalarm.Old.WinPhone.Resources;
using Pollenalarm.Old.WinPhone.Models;
using Pollenalarm.Old.WinPhone.ViewModels;
using Pollenalarm.Old.WinPhone.Controls;
using Pollenalarm.Old.WinPhone.Helper;

namespace Pollenalarm.Old.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Progress Indicator
        public static ProgressIndicator progressIndicator;
        public static bool firstTime = true;
        public static bool mapLoaded = false;
        private RemovableAdvert removableAdvert;

        // ------------------------------------------------------------------------------
        // Constructor
        // ------------------------------------------------------------------------------

        public MainPage()
        {
            InitializeComponent();

            // Register shake event
            //ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);
            //ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 7;
            //ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = 3;
            //ShakeGesturesHelper.Instance.Active = true;

            // Advertisement
            //removableAdvert = new RemovableAdvert("13a910fb-97d2-4f56-8390-c184e8c77cdd", "10035173", App.hideTrialsOnFullVersion || App.IsPromo);
            //AdPanel.Children.Add(removableAdvert);

            // Initialize variables
            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);

            // Check first start and Updates
            CheckUpdate("2.2");
            CheckFirstStart();

            // Backgound Task
            if (App.CheckSettings("BackgroundTask"))
            {
                StartPeriodicAgent();
            }

            // Load Data
            map.Visibility = Visibility.Collapsed;
            places.Visibility = Visibility.Collapsed;
            Loading.Visibility = Visibility.Visible;

            MainViewModel.MapDownloadCompleted += new MainViewModel.DownloadedCompletedHandler(MainViewModel_MapDownloadCompleted);
            MainViewModel.Current.LoadData();

        }

        void MainViewModel_MapDownloadCompleted(object sender, EventArgs e)
        {
            mapLoaded = true;
            ShellProgessStop();

            map.Visibility = Visibility.Visible;
            places.Visibility = Visibility.Visible;
            Loading.Visibility = Visibility.Collapsed;
            AnimationPopMapPlaces.Begin();
        }

        // ------------------------------------------------------------------------------
        // Navigation Events
        // ------------------------------------------------------------------------------

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Begin();

            // Add Advertising
            //removableAdvert.AddAdControl();

            CheckWarning();
            //CheckLicense();

            CheckSettings();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            AnimationEnd.Begin();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Remove Advertising
            //removableAdvert.RemoveAdControl();
        }

        // ------------------------------------------------------------------------------
        // Start Methods
        // ------------------------------------------------------------------------------

        private void CheckFirstStart()
        {
            if (!App.Settings.Contains("FirstStart") || (bool)App.LoadFromSettings("FirstStart", null) == true)
            {
                ShowFirstStart();
            }
        }

        private void CheckUpdate(string update)
        {
            if (!App.Settings.Contains(update) || (bool)App.LoadFromSettings(update, null) == true)
            {
                switch (update)
                {
                    case "1.3":
                        // Delete All Pollen
                        if (App.Settings.Contains("AllPollen"))
                        {
                            App.Settings.Remove("AllPollen");
                            MessageBox.Show("Aufgrund einiger Änderungen beim letzten Update wurde Ihre Pollenauswahl zurückgesetzt. Bitte wählen Sie erneut aus, auf welche Pollen Sie allergisch reagieren!", "Update Änderung", MessageBoxButton.OK);
                        }

                        App.SaveToSettings("1.3", false);
                        break;
                    default:
                        break;
                }

                App.Settings.Save();
            }
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

            // Shake To Refresh
            if (!Accelerometer.IsSupported)
            {
                tbkShake.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (App.Settings.Contains("ShakeToRefresh"))
                {
                    if (((bool)App.Settings["ShakeToRefresh"]) == true)
                    {
                        tbkShake.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        tbkShake.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    tbkShake.Visibility = Visibility.Visible;
                }
            }
        }

        private void CheckLicense()
        {
            LicenseInformation license = new LicenseInformation();
            if (license.IsTrial() == false)
            {
                AdPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CheckWarning()
        {
            if (MainViewModel.Current.AllPlaces.Count > 0)
            {
                tbkShowHelp.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbkShowHelp.Visibility = Visibility.Visible;
            }
        }

        private void ShowFirstStart()
        {
            App.SaveToSettings("FirstStart", false);

            // Activate Background Task
            App.SaveToSettings("BackgroundTask", true);

            // Activate Location Services
            App.SaveToSettings("LocationServicesAllowed", true);

            // Hide Advertising on MainPage
            AdPanel.Children.Remove(removableAdvert);

            if (App.Settings.Contains("AllPollen"))
            {
                App.Settings.Remove("AllPollen");
            }

            App.Settings.Save();
        }

        //private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        //{
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        if (App.Settings.Contains("ShakeToRefresh"))
        //        {
        //            if (((bool)App.Settings["ShakeToRefresh"]) == false)
        //            {
        //                return;
        //            }
        //        }

        //        // Refresh Data
        //        AnimationRefresh.Begin();
        //        AnimationWind.Begin();
        //        MainViewModel.Current.Refresh();
        //        ShellProgessStop();
        //    });
        //}

        // ------------------------------------------------------------------------------
        // Background Agent 
        // ------------------------------------------------------------------------------

        public static void StartPeriodicAgent()
        {
            if (LowMemoryHelper.IsLowMemDevice)
                return;

            var periodicTask = new PeriodicTask("TileUpdaterTask")
            {
                Description = "Die Polleninformationen Ihrer Live Tiles werden im eingestellten Intervall aktualisiert."
            };

            if (ScheduledActionService.Find(periodicTask.Name) != null)
            {
                StopPeriodicAgent();
            }
            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch (InvalidOperationException)
            {

            }
        }

        public static void StopPeriodicAgent()
        {
            try
            {
                ScheduledActionService.Remove("TileUpdaterTask");
            }
            catch (Exception)
            {
                Debug.WriteLine("Fehler. Es wurde keine Hintergrundaufgabe gefunden.");
            }
        }

        // ------------------------------------------------------------------------------
        // Progress Indicator Methods
        // ------------------------------------------------------------------------------

        public static void ShellProgressStart(String text)
        {
            progressIndicator.IsVisible = true;
            progressIndicator.IsIndeterminate = true;
            progressIndicator.Text = text;
        }

        public static void ShellProgessStop()
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;
        }

        // ------------------------------------------------------------------------------
        // GUI Events
        // ------------------------------------------------------------------------------

        // --------------------------------------
        // Application Bar
        // --------------------------------------

        // Add place
        private void appBarAddPlace_Click(object sender, EventArgs e)
        {
            // Set current Place null to create a new one on Place Add/Edit Page
            MainViewModel.Current.CurrentPlace = null;

            // Navigate to Place Add/Edit Page
            NavigationService.Navigate(new Uri("/Pages/PlaceAddEdit.xaml", UriKind.Relative));
        }

        // Settings
        private void appBarSettings_Click(object sender, EventArgs e)
        {
            // Navigate to Place Settings Page
            NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
        }

        // Select pollen
        private void appBarPollenSelection_Click(object sender, EventArgs e)
        {
            // Navigate to Pollen Selection
            NavigationService.Navigate(new Uri("/Pages/PollenSelection.xaml", UriKind.Relative));
        }

        // Help and Information
        private void appBarHelp_Click(object sender, EventArgs e)
        {
            // Navigate to Help and Information Selection
            NavigationService.Navigate(new Uri("/Pages/HelpAndInformation.xaml", UriKind.Relative));
        }

        // Refresh Data
        private void appBarRefresh_Click(object sender, EventArgs e)
        {
            AnimationRefresh.Begin();
            MainViewModel.Current.Refresh();
            ShellProgessStop();
        }

        // --------------------------------------
        // General GUI elements
        // --------------------------------------

        private void spnGeneralInformation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AboutPrompt about = new AboutPrompt();
            about.Title = "Informationen";
            about.VersionNumber = " ";
            about.Body = new TextBlock { Text = MainViewModel.Current.GeneralInformation, TextWrapping = TextWrapping.Wrap, FontSize = 22 };
            about.ActionPopUpButtons[0].Click += new RoutedEventHandler(MainPage_Click);
            about.Show();

            SystemTray.Opacity = 0.65;
        }

        // --------------------------------------
        // List Box for Places
        // --------------------------------------

        // Menu edit
        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = sender as MenuItem;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;
                MainViewModel.Current.CurrentPlace = currentPlace;

                // Navigate to Place Add/Edit Page
                NavigationService.Navigate(new Uri("/Pages/PlaceAddEdit.xaml", UriKind.Relative));
            }
        }

        // Menu delete
        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = sender as MenuItem;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;

                // Remove currentPlace
                currentPlace.DeleteTile();
                MainViewModel.Current.AllPlaces.Remove(currentPlace);
                CheckWarning();

            }
        }

        // Menu Pin To Start
        private void menuPinToStart_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = sender as MenuItem;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;

                // Pin current Place To Start
                currentPlace.PinToStart();
            }
        }

        // Place Details
        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border s = sender as Border;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;
                MainViewModel.Current.CurrentPlace = currentPlace;

                // Navigate to Place Details Page
                NavigationService.Navigate(new Uri("/Pages/PlaceDetails.xaml", UriKind.Relative));
            }
        }

        // --------------------------------------
        // Miscellaneous
        // --------------------------------------

        // Check whether current Place is pinned to start and set 'Pin To Start' to enabled or disabled
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu s = sender as ContextMenu;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;

                if (currentPlace.IsCurrentPosition == false)
                {
                    // Check whether Tile is pinned
                    ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("id=" + currentPlace.ID));
                    if (TileToFind != null)
                    {
                        ((MenuItem)s.Items[0]).IsEnabled = false;
                    }
                    else
                    {
                        ((MenuItem)s.Items[0]).IsEnabled = true;
                    }
                }
                else
                {
                    s.IsOpen = false;
                }
            }
        }

        void MainPage_Click(object sender, RoutedEventArgs e)
        {
            SystemTray.Opacity = 0.001;
        }

        private void appBarTestOfflineUpdate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Start Background Task now!");
            ScheduledActionService.LaunchForTest("TileUpdaterTask", new TimeSpan(0, 0, 5));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void pivot_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;

                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromSeconds(0.5);

                dt.Tick += delegate { AnimationWind.Begin(); dt.Stop(); };
                dt.Start();
            }
        }

        private void pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // TODO: Add event handler implementation here.

            if (pivot.SelectedIndex == 1)
            {
                if (mapLoaded == false)
                {
                    ShellProgressStart("Kartendaten werden geladen.");
                }
                else
                {
                    AnimationPopMapPlaces.Begin();
                }
            }
            else
            {
                AnimationMapFadeOut.Begin();
            }
        }

        private void appBarSearch_Click(object sender, System.EventArgs e)
        {
            // Navigate to Help and Information Selection
            NavigationService.Navigate(new Uri("/Pages/Search.xaml", UriKind.Relative));
        }

        private void cbbBerlin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place berlin = new Place(App.GetNextPlaceID(), "Berlin", "10115");
            MainViewModel.Current.CurrentPlace = berlin;
            berlin.LoadingFinished += berlin_LoadingFinished;
            berlin.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        void berlin_LoadingFinished(object sender)
        {
            LoadingInformation.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Uri("/Pages/PlaceDetails.xaml", UriKind.Relative));
        }

        private void cbbBonn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Bonn", "53111");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbDresden_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Dresden", "01067");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbFrankfurt_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Frankfurt", "60311");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbHamburg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Hamburg", "20095");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbHannover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Hannover", "30159");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbMuenchen_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "München", "80331");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbNuernberg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Nürnberg", "90402");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbSaarbruecken_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Saarbrücken", "66111");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbStuttgart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Stuttgart", "70173");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }

        private void cbbRostock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Place p = new Place(App.GetNextPlaceID(), "Rostock", "18055");
            MainViewModel.Current.CurrentPlace = p;
            p.LoadingFinished += berlin_LoadingFinished;
            p.DownloadPollenList();
            LoadingInformation.Visibility = Visibility.Visible;
        }
    }
}