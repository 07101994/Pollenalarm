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
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using Pollenalarm.Old.WinPhone.ViewModels;
using Pollenalarm.Old.WinPhone.Models;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class Search : PhoneApplicationPage
    {
        public static ProgressIndicator progressIndicator;
        private bool isSearchFromAddPlace = false;

        public Search()
        {
            InitializeComponent();


            pivotSearchResults.Opacity = 0;

            //MainViewModel.Current.SearchResultPlaces = new ObservableCollection<Place>();

            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.SearchResultPlaces.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SearchResultPlaces_CollectionChanged);
            MainViewModel.Current.SearchResultPollen.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SearchResultPollen_CollectionChanged);
        }

        void SearchResultPollen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (MainViewModel.Current.SearchResultPollen.Count == 0)
            {
                tbkNoPollen.Visibility = Visibility.Visible;
            }
            else
            {
                tbkNoPollen.Visibility = Visibility.Collapsed;
            }
        }

        void SearchResultPlaces_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (MainViewModel.Current.SearchResultPlaces.Count == 0)
            {
                tbkNoPlaces.Visibility = Visibility.Visible;
            }
            else
            {
                tbkNoPlaces.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Begin();
            CheckSettings();

            if (this.NavigationContext.QueryString.ContainsKey("search"))
            {
                isSearchFromAddPlace = true;

                if (this.NavigationContext.QueryString["search"].Equals("null") == false)
                {
                    tbxSearch.Text = this.NavigationContext.QueryString["search"];
                    SearchText(tbxSearch.Text);
                }
                else
                {
                    tbxSearch.Focus();
                }
            }
            else
            {
                isSearchFromAddPlace = false;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            AnimationEnd.Begin();
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

        private void SearchText(string search)
        {
            this.Focus();
            ShellProgressStart("Suche nach " + search + ".");

            if (pivotSearchResults.Opacity != 100)
            {
                AnimationShowResults.Begin();
            }

            // Clear Result Collections
            MainViewModel.Current.SearchResultPlaces.Clear();
            MainViewModel.Current.SearchResultPollen.Clear();

            // ----------------------------------------
            // Pollen
            // ----------------------------------------
            bool pollenFound = false;
            foreach (Pollen pollen in MainViewModel.Current.AllPollen)
            {
                if (pollen.Name.ToUpper().Contains(search.ToUpper().Trim()) || search.ToUpper().Trim().Contains(pollen.Name.ToUpper()))
                {
                    MainViewModel.Current.SearchResultPollen.Add(pollen);
                    pollenFound = true;                    
                }
            }

            if (pollenFound)
                pivotSearchResults.SelectedIndex = 1;
            else
                pivotSearchResults.SelectedIndex = 0;

            // ----------------------------------------
            // Cityname or Postal Code
            // ----------------------------------------
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                GeocodeQuery geocodeQuery = new GeocodeQuery();
                geocodeQuery.GeoCoordinate = new GeoCoordinate(51.5167, 9.9167); //Germany
                geocodeQuery.SearchTerm = search.Trim();
                geocodeQuery.QueryCompleted += GeocodeQuery_QueryCompleted;
                geocodeQuery.QueryAsync();


                //// Create GeoCode Request to get City-Location
                //GeocodeRequest geocodeRequest = new GeocodeRequest();
                //geocodeRequest.Credentials = new MyBingMaps.Credentials();
                //geocodeRequest.Credentials.ApplicationId = "Aq69-RXbmx-nbDdlZISg__ioXRQH29f2-jgseV9r1acfqU6jFyCWsPfjANiy4o76";

                //// Set Adress name to search text
                //Address address = new Address();

                //// Check whether Cityname or Postal Code
                //int testPlz;
                //if (search.Trim().Length == 5 && int.TryParse(search.Trim(), out testPlz))
                //{
                //    address.PostalCode = search.Trim();
                //}
                //else
                //{
                //    address.Locality = search.Trim();
                //}
                //address.CountryRegion = "DE";
                //geocodeRequest.Address = address;

                //// Send rquest
                //GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                //geocodeService.GeocodeCompleted += new EventHandler<GeocodeCompletedEventArgs>(geocodeService_GeocodeCompleted);
                //geocodeService.GeocodeAsync(geocodeRequest);
            }
            else
            {
                ShellProgessStop();
            }
        }


        private Queue<MapLocation> searchResultQueue;
        private ReverseGeocodeQuery reverseGeocodeQuery;
        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            // Create Reverse GeoCode Request to get ZIP Code from Loacation
            //ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();
            //reverseGeocodeRequest.Credentials = new MyBingMaps.Credentials();
            //reverseGeocodeRequest.Credentials.ApplicationId = "Aq69-RXbmx-nbDdlZISg__ioXRQH29f2-jgseV9r1acfqU6jFyCWsPfjANiy4o76";

            if (e.Result.Any())
            {
                // Fill queue
                searchResultQueue = new Queue<MapLocation>(e.Result.Count);
                for (var i = 0; i < e.Result.Count; i++)
                    searchResultQueue.Enqueue(e.Result.ElementAt(i));

                reverseGeocodeQuery = new ReverseGeocodeQuery();
                reverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;

                // Work queue                
                var enqueuedResult = searchResultQueue.Dequeue();
                reverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(enqueuedResult.GeoCoordinate.Latitude, enqueuedResult.GeoCoordinate.Longitude);
                reverseGeocodeQuery.QueryAsync();
            }
            else
            {
                ShellProgessStop();
            }
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Result.First().Information.Address.City.Length > 0)
            {
                Place p = new Place();
                p.ID = App.GetNextPlaceID();
                p.Name = e.Result.First().Information.Address.City;
                p.Plz = e.Result.First().Information.Address.PostalCode;
                p.DownloadPollenList();

                MainViewModel.Current.SearchResultPlaces.Add(p);
            }

            // Remove current query from queue
            if (searchResultQueue.Count > 0)
            {
                var enqueuedResult = searchResultQueue.Dequeue();
                reverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(enqueuedResult.GeoCoordinate.Latitude, enqueuedResult.GeoCoordinate.Longitude);
                reverseGeocodeQuery.QueryAsync();
            }

            ShellProgessStop();
        }

        private void tbxSearch_ActionIconTapped(object sender, EventArgs e)
        {
            if (tbxSearch.Text.Length > 0)
            {
                SearchText(tbxSearch.Text);
            }
        }

        private void tbxSearch_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbxSearch.Text.Length > 0)
            {
                SearchText(tbxSearch.Text);
            }
        }

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

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid s = sender as Grid;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;
                MainViewModel.Current.CurrentPlace = currentPlace;

                if (isSearchFromAddPlace)
                {
                    NavigationService.RemoveBackEntry();
                    NavigationService.Navigate(new Uri("/Views/PlaceAddEdit.xaml?SaveFromSearch=true", UriKind.Relative));                    
                }
                else
                {
                    // Navigate to Place Details Page
                    NavigationService.Navigate(new Uri("/Views/PlaceDetails.xaml", UriKind.Relative));
                    NavigationService.RemoveBackEntry();
                }
            }
        }

        private void GridPollenResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid s = sender as Grid;

            if (s != null)
            {
                // Set currentPlace to selected place
                Pollen currentPollen = s.DataContext as Pollen;
                MainViewModel.Current.CurrentPollen = currentPollen;

                // Navigate to Place Details Page
                NavigationService.Navigate(new Uri("/Views/PollenDetails.xaml", UriKind.Relative));
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = sender as MenuItem;

            if (s != null)
            {
                // Set currentPlace to selected place
                Place currentPlace = s.DataContext as Place;
                MainViewModel.Current.CurrentPlace = currentPlace;

                // Navigate to Place Add/Edit Page
                NavigationService.RemoveBackEntry();
                NavigationService.Navigate(new Uri("/Views/PlaceAddEdit.xaml?SaveFromSearch=true", UriKind.Relative));
            }
        }

        private void tbxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbxSearch.Text.Length > 0)
                {
                    SearchText(tbxSearch.Text);
                }
            }
        }
    }
}