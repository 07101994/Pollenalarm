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
using System.Device.Location;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Pollenalarm.Old.WinPhone.ViewModels;
using Pollenalarm.Old.WinPhone.Models;
using Microsoft.Phone.Maps.Services;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class PlaceAddEdit : PhoneApplicationPage
    {
        private bool isNewPlace;
        GeoCoordinateWatcher watcher;
        public static ProgressIndicator progressIndicator;
        //private RemovableAdvert removableAdvert;
        private bool isPlaceToCheck = false;

        public PlaceAddEdit()
        {
            InitializeComponent();

            //removableAdvert = new RemovableAdvert("13a910fb-97d2-4f56-8390-c184e8c77cdd", "10035157", App.hideTrialsOnFullVersion || App.IsPromo);
            //AdPanel.Children.Add(removableAdvert);

            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Begin();

            // ------------------------------
            // Check Settings
            // ------------------------------
            if (this.NavigationContext.QueryString.ContainsKey("SaveFromSearch") && this.NavigationContext.QueryString["SaveFromSearch"].ToLower().Equals("true"))
            {
                // ------------------------------
                // Add an new Place from Search
                // ------------------------------                
                NavigationService.RemoveBackEntry();

                isNewPlace = true;

                // Fill fields
                tbxName.Text = MainViewModel.Current.CurrentPlace.Name;
                tbxPlz.Text = MainViewModel.Current.CurrentPlace.Plz;

            }
            else if (MainViewModel.Current.CurrentPlace == null)
            {
                // ------------------------------
                // Add an new Place from scratch
                // ------------------------------
                isNewPlace = true;
            }
            else
            {
                // ------------------------------
                // Edit an existig Place
                // -----------------------------
                isNewPlace = false;

                // Change Page Title
                PageTitle.Text = "Ort bearbeiten";

                // Add Remove Icon to Application Bar
                ApplicationBarMenuItem deleteButton = new ApplicationBarMenuItem();
                deleteButton.Text = "Löschen";
                deleteButton.Click += new EventHandler(deleteButton_Click);
                ApplicationBar.MenuItems.Add(deleteButton);

                // Fill fields
                tbxName.Text = MainViewModel.Current.CurrentPlace.Name;
                tbxPlz.Text = MainViewModel.Current.CurrentPlace.Plz;
            }
          
            CheckSettings();

            // Add Advertising
            //removableAdvert.AddAdControl();
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

            // Check Place on Map
            if (App.Settings.Contains("CheckPlace"))
            {
                if (((bool)App.Settings["CheckPlace"]) == true)
                {
                    isPlaceToCheck = true;
                    cbCheckPlace.IsChecked = false;
                }
                else
                {
                    isPlaceToCheck = false;
                }
            }
      
            // Location permission
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.Start();

            if (watcher.Permission == GeoPositionPermission.Denied)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            AnimationEnd.Begin();

            
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {            
            // Remove Advertising
            //removableAdvert.RemoveAdControl();
        }

        // ------------------------------
        // GUI Events
        // -----------------------------

        // Save Button clicked
        private void appBarSave_Click(object sender, EventArgs e)
        {
            //------------------------------------
            // Check values
            //------------------------------------

            // Check whether all fields are filles
            if (tbxName.Text.Length <= 0 || tbxPlz.Text.Length <= 0)
            {
                MessageBox.Show("Bitte füllen Sie alle Felder mit korrekten Werten aus!", "Unvollständige Angaben", MessageBoxButton.OK);
                return;
            }

            // Check whether place already exits
            bool isAlreadyExistant = false;
            foreach (Place p in MainViewModel.Current.AllPlaces)
            {
                if (p.IsCurrentPosition == false && p.Plz.Equals(tbxPlz.Text.TrimEnd()))
                {
                    // Only ignore ignore same ID at edited places because new places doesn't have one at this moment.
                    if (isNewPlace == false)
                    {
                        if (p.ID != MainViewModel.Current.CurrentPlace.ID)
                        {
                            isAlreadyExistant = true;
                        }
                    }
                    else
                    {
                        isAlreadyExistant = true;
                    }
                }
            }

            if (isAlreadyExistant)
            {
                MessageBox.Show("Sie behalten diesen Ort bereits im Auge. Bitte wählen Sie eine andere Postleitzahl!", "Ort existiert bereits", MessageBoxButton.OK);
                return;
            }

            // Disable all Application Bar Buttons
            foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
            {
                button.IsEnabled = false;
            }

            if (isPlaceToCheck)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    GeocodeQuery geocodeQuery = new GeocodeQuery();
                    geocodeQuery.GeoCoordinate = new GeoCoordinate(51.5167, 9.9167); //Germany
                    geocodeQuery.SearchTerm = tbxPlz.Text;
                    geocodeQuery.QueryCompleted += GeocodeQuery_QueryCompleted;
                    geocodeQuery.QueryAsync();

                    //GeocodeRequest geocodeRequest = new GeocodeRequest();
                    //geocodeRequest.Credentials = new MyBingMaps.Credentials();
                    //geocodeRequest.Credentials.ApplicationId = "Aq69-RXbmx-nbDdlZISg__ioXRQH29f2-jgseV9r1acfqU6jFyCWsPfjANiy4o76";

                    //Address address = new Address();
                    //address.PostalCode = tbxPlz.Text;
                    //address.CountryRegion = "DE";
                    //geocodeRequest.Address = address;

                    //GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                    //geocodeService.GeocodeCompleted += new EventHandler<GeocodeCompletedEventArgs>(geocodeService_GeocodeCompleted);
                    //geocodeService.GeocodeAsync(geocodeRequest);
                }
                else
                {
                    CreateOrEditPlace();
                }
            }
            else
            {
                CreateOrEditPlace();
            }
        }

        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Result.Count < 1)
            {
                MessageBox.Show("Diese Postleitzahl existiert leider nicht. Bitte geben Sie eine andere Postleitzahl ein.", "Fehlerhafte PLZ", MessageBoxButton.OK);
                return;
            }

            // Show 
            if (isPlaceToCheck)
            {
                AnimationOverlay.Begin();
                this.Focus();
            }

            GeoCoordinate geoCoordinate = new GeoCoordinate(e.Result.First().GeoCoordinate.Latitude, e.Result.First().GeoCoordinate.Longitude);
            MapViewer.LocationToViewportPoint(geoCoordinate);
            MapViewer.SetView(geoCoordinate, 12);
        }

        private void CreateOrEditPlace()
        {
            //------------------------------------
            // Create or edit place
            //------------------------------------

            if (isNewPlace)
            {
                if (App.License.IsTrial() == true && App.IsPromo == false)
                {
                    int placeLimit = 2;

                    if (App.CurrentPositionPlace == null)
                        placeLimit = 1;

                    if (MainViewModel.Current.AllPlaces.Count >= placeLimit)
                    {
                        App.ShowBuyMessage("Sie können in der Testversion nur einen Ort speichern. Kaufen Sie die Vollversion, um den vollen Funktionsumfang nutzen zu können.");
                        foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                        {
                            button.IsEnabled = true;
                        }
                        return;
                    }
                }
                
                // Create new Place to save
                Place placeToSave = new Place();

                // Set ID, Name and PLZ
                placeToSave.ID = App.GetNextPlaceID();
                placeToSave.Name = tbxName.Text;
                placeToSave.Plz = tbxPlz.Text; //TODO: Check whether PLZ is valid

                // Add Place to Placelist and Download Pollen Information
                placeToSave.DownloadPollenList();
                MainViewModel.Current.AllPlaces.Add(placeToSave);
            }
            else
            {
                // Edit Place
                bool isNameChanged = false;

                // Download new Polleninformation if PLZ has changed
                if (!MainViewModel.Current.CurrentPlace.Plz.Equals(tbxPlz.Text))
                {
                    MainViewModel.Current.CurrentPlace.DownloadPollenList();
                }

                // Create new Image if Name has changed
                if (!MainViewModel.Current.CurrentPlace.Name.Equals(tbxName.Text))
                {
                    isNameChanged = true;
                }

                MainViewModel.Current.CurrentPlace.Name = tbxName.Text;
                MainViewModel.Current.CurrentPlace.Plz = tbxPlz.Text;

                // Update Live Tile
                MainViewModel.Current.CurrentPlace.UpdateLiveTile(isNameChanged);
            }

            // Navigate Back
            NavigationService.GoBack();
        }

        // Cancel
        private void appBarCancel_Click(object sender, EventArgs e)
        {
            MainViewModel.Current.CurrentPlace = null;
            NavigationService.GoBack();
        }

        // Delete
        void deleteButton_Click(object sender, EventArgs e)
        {
            MainViewModel.Current.AllPlaces.Remove(MainViewModel.Current.CurrentPlace);
            MainViewModel.Current.CurrentPlace.DeleteTile();
            MainViewModel.Current.CurrentPlace = null;            
            NavigationService.GoBack();
        }

        // Get current Location
        private void appBarLocation_Click(object sender, EventArgs e)
        {
            GetCurrentLocation();
        }

        private void GetCurrentLocation()
        {
            if (App.CheckSettings("LocationServicesAllowed"))
            {
                // Start Progress Bar
                progressIndicator.IsVisible = true;
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Aktuelle Position wird ermittelt.";

                GetCurrentZipCode();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Für diese Funktion wird der Zugriff auf Ihre aktuelle Position benötigt.\nDer Verwendung dieser Daten haben Sie bisher nicht zugestimmt.\n\nWählen Sie \"Ok\" um den Zugriff auf Ihre aktuelle Position zu gewähren. Dies kann jederzeit in den Einstellungen rückgängig gemacht werden.", "Zugriff auf die aktuelle Position erlauben?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    App.SaveToSettings("LocationServicesAllowed", true);
                    MainViewModel.Current.AddCurrentPosition();
                    GetCurrentLocation();
                }
            }
        }

        public void GetCurrentZipCode()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); // using high accuracy
                watcher.MovementThreshold = 20; // use MovementThreshold to ignore noise in the signa

                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged +=new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                watcher.Start();
            }
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        Debug.WriteLine("you have this application access to location.");
                    }
                    else
                    {
                        Debug.WriteLine("location is not functioning on this device");
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    Debug.WriteLine("location data is not available.");
                    
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    Debug.WriteLine("location data is available.");
                    
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            watcher.Stop();
            watcher.Dispose();
            watcher = null;

            ReverseGeocodeQuery reverseGeocodeQuery = new ReverseGeocodeQuery();
            reverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            reverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
            reverseGeocodeQuery.QueryAsync();


            //ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();

            //// Set the credentials using a valid Bing Maps key
            //reverseGeocodeRequest.Credentials = new MyBingMaps.Credentials();
            //reverseGeocodeRequest.Credentials.ApplicationId = "Aq69-RXbmx-nbDdlZISg__ioXRQH29f2-jgseV9r1acfqU6jFyCWsPfjANiy4o76";

            //MyBingMaps.Location point = new MyBingMaps.Location();
            //point.Latitude = e.Position.Location.Latitude;
            //point.Longitude = e.Position.Location.Longitude;

            //reverseGeocodeRequest.Location = point;

            //// Make the reverse geocode request
            //GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            //geocodeService.ReverseGeocodeCompleted += new EventHandler<ReverseGeocodeCompletedEventArgs>(geocodeService_ReverseGeocodeCompleted);
            //geocodeService.ReverseGeocodeAsync(reverseGeocodeRequest);
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;

            tbxName.Text = e.Result[0].Information.Address.City;
            tbxPlz.Text = e.Result[0].Information.Address.PostalCode;
        }

        private void btnCancelPlace_Click(object sender, RoutedEventArgs e)
        {
            AnimationOverlayClose.Begin();
            // Disable all Application Bar Buttons
            foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
            {
                button.IsEnabled = true;
            }
        }

        private void btnApplyPlace_Click(object sender, RoutedEventArgs e)
        {
            CreateOrEditPlace();
        }

        private void cbCheckPlace_Checked(object sender, RoutedEventArgs e)
        {
            App.SaveToSettings("CheckPlace", false);
            isPlaceToCheck = false;
        }

        private void cbCheckPlace_Unchecked(object sender, RoutedEventArgs e)
        {
            App.SaveToSettings("CheckPlace", true);
            isPlaceToCheck = true;
        }


        private void appBarSearch_Click(object sender, System.EventArgs e)
        {
            if (tbxName.Text.Length != 0)
            {
                NavigationService.Navigate(new Uri("/Views/Search.xaml?search=" + tbxName.Text, UriKind.Relative));
            }
            else if (tbxPlz.Text.Length != 0)
            {
                NavigationService.Navigate(new Uri("/Views/Search.xaml?search=" + tbxPlz.Text, UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Search.xaml?search=null", UriKind.Relative));
            }
        }
        
    }        
}