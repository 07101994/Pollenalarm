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
using Microsoft.Phone.Marketplace;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Pollenalarm.Old.WinPhone.Controls;
using Pollenalarm.Old.WinPhone.ViewModels;
using Pollenalarm.Old.WinPhone.Models;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class PlaceDetails : PhoneApplicationPage
    {
        //private RemovableAdvert removableAdvert;
        ConcentrationBar concentrationBar;

        public PlaceDetails()
        {
            InitializeComponent();
            concentrationBar = new ConcentrationBar(MainViewModel.Current.CurrentPlace.AverageConcentrationInt);
            this.spnHeader.Children.Add(concentrationBar);

            //removableAdvert = new RemovableAdvert("13a910fb-97d2-4f56-8390-c184e8c77cdd", "10035159", App.hideTrialsOnFullVersion || App.IsPromo);
            //AdPanel.Children.Add(removableAdvert);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void CheckNoPollenWarning()
        {
            List<Pollen> todayList;
            List<Pollen> tomorrowList;
            List<Pollen> afterTodayList;

            if (App.ShowAllPollen)
            {
                todayList = MainViewModel.Current.CurrentPlace.PollenList.Where(x => x.IsSelected == true).ToList();
                tomorrowList = MainViewModel.Current.CurrentPlace.PollenListTomorrow.Where(x => x.IsSelected == true).ToList();
                afterTodayList = MainViewModel.Current.CurrentPlace.PollenListDayAfterTomorrow.Where(x => x.IsSelected == true).ToList();
            }
            else
            {
                todayList = MainViewModel.Current.CurrentPlace.PollenList.Where(x => x.IsSelected == true && x.Concentration > Concentration.None).ToList();
                tomorrowList = MainViewModel.Current.CurrentPlace.PollenListTomorrow.Where(x => x.IsSelected == true && x.Concentration > Concentration.None).ToList();
                afterTodayList = MainViewModel.Current.CurrentPlace.PollenListDayAfterTomorrow.Where(x => x.IsSelected == true && x.Concentration > Concentration.None).ToList();
            }


            // Show alway because Buy Message is only displayed on Tomorrow and Day After Tomorrow
            if (todayList.Count == 0)
                tbkNoPollenToday.Visibility = Visibility.Visible;
            else
                tbkNoPollenToday.Visibility = Visibility.Collapsed;

            // Hide if Buy Message is displayed
            if (App.License.IsTrial() == false)
            {
                if (tomorrowList.Count == 0)
                    tbkNoPollenTomorrow.Visibility = Visibility.Visible;
                else
                    tbkNoPollenTomorrow.Visibility = Visibility.Collapsed;

                if (afterTodayList.Count == 0)
                    tbkNoPollenAfterTomorrow.Visibility = Visibility.Visible;
                else
                    tbkNoPollenAfterTomorrow.Visibility = Visibility.Collapsed;
            }
        }     

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Completed += new EventHandler(AnimationStart_Completed);
            AnimationStart.Begin();

            concentrationBar.Reset();
            CheckNoPollenWarning();

            MainViewModel.Current.CurrentPlace.LoadingFinished += CurrentPlace_LoadingFinished;
            this.tbkName.Text = MainViewModel.Current.CurrentPlace.Name;
            this.tbkPlz.Text = MainViewModel.Current.CurrentPlace.Plz;            

            if (MainViewModel.Current.CurrentPlace.IsCurrentPosition == false)
            {
                appBarPin.Visibility = Visibility.Visible;

                // Check whether Tile is pinned
                ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("id=" + MainViewModel.Current.CurrentPlace.ID));
                if (TileToFind != null)
                {
                    appBarPin.IsEnabled = false;
                }
                else
                {
                    appBarPin.IsEnabled = true;
                }
            }
            else
            {
                appBarPin.Visibility = Visibility.Collapsed;
            }

            CheckSettings();

            // Add Advertising
            //removableAdvert.AddAdControl();

            if (MainViewModel.Current.AllPlaces.Contains(MainViewModel.Current.CurrentPlace))
            {
                appBarSave.Visibility = Visibility.Collapsed;
            }

            if (App.License.IsTrial() == true && App.IsPromo == false)
            {
                // Show Buy Message
                spnBuyTomorrow.Visibility = Visibility.Visible;
                spnBuyAfterTomorrow.Visibility = Visibility.Visible;

                // Hide Pollen Information
                lbxTomorrow.Visibility = Visibility.Collapsed;
                lbxAfterTomorrow.Visibility = Visibility.Collapsed;

                tbkNoPollenTomorrow.Visibility = Visibility.Collapsed;
                tbkNoPollenAfterTomorrow.Visibility = Visibility.Collapsed;
            }
            else
            {
                spnBuyTomorrow.Visibility = Visibility.Collapsed;
                spnBuyAfterTomorrow.Visibility = Visibility.Collapsed;

                lbxTomorrow.Visibility = Visibility.Visible;
                lbxAfterTomorrow.Visibility = Visibility.Visible;
            }


            //CheckLicense();
        }

        void CurrentPlace_LoadingFinished(object sender)
        {
            CheckNoPollenWarning();
            this.spnHeader.Children.Remove(concentrationBar);
            concentrationBar = new ConcentrationBar(MainViewModel.Current.CurrentPlace.AverageConcentrationInt);            
            this.spnHeader.Children.Add(concentrationBar);
            concentrationBar.Animate();
        }

        void AnimationStart_Completed(object sender, EventArgs e)
        {
            concentrationBar.Animate();
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

        private void CheckLicense()
        {
            LicenseInformation license = new LicenseInformation();
            if (license.IsTrial() == false)
            {
                AdPanel.Visibility = Visibility.Collapsed;
            }
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

        private void appBarPin_Click(object sender, EventArgs e)
        {
            // Add Place to All Places List if it doesn't exist there (e.g. for search)
            if (MainViewModel.Current.AllPlaces.Contains(MainViewModel.Current.CurrentPlace) == false)
            {
                MainViewModel.Current.AllPlaces.Add(MainViewModel.Current.CurrentPlace);
            }

            MainViewModel.Current.CurrentPlace.PinToStart();

            //// Create Tile Background
            //Rectangle rect = new Rectangle();
            //rect.Height = 173;
            //rect.Width = 173;
            //rect.Fill = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];

            //// Place Name
            //TextBlock tbkName = new TextBlock();
            //tbkName.Text = MainViewModel.Current.CurrentPlace.NameUpper;
            //tbkName.FontFamily = new FontFamily("Segoe UI");
            //tbkName.Foreground = new SolidColorBrush(Colors.White);
            //tbkName.FontSize = 24;
            //tbkName.Padding = new Thickness(7, 135, 7, 0);
            //tbkName.FontWeight = FontWeights.SemiBold;

            //// Concentration Header
            //TextBlock txbHeader = new TextBlock();
            //txbHeader.Text = "Belastung";
            //txbHeader.FontFamily = new FontFamily("Segoe WP");
            //txbHeader.Foreground = new SolidColorBrush(Colors.White);
            //txbHeader.FontSize = 19;
            //txbHeader.Padding = new Thickness(7, 74, 0, 0);

            //// Border for Concentration Bar
            //Rectangle border = new Rectangle();
            //border.Height = 19;
            //border.Width = 120;
            //border.Stroke = new SolidColorBrush(Colors.White);
            //border.StrokeThickness = 2;
            //border.Margin = new Thickness(7, 105, 0, 0);

            //// ConcentrationBar
            //Rectangle cBar = new Rectangle();
            //cBar.Height = 11;
            //cBar.Width = 80; // 40 - 80 - 112
            //cBar.Fill = new SolidColorBrush(Colors.White);
            //cBar.StrokeThickness = 0;
            //cBar.Margin = new Thickness(11, 109, 0, 0);

            //Image img = new Image();
            //img.Source = new BitmapImage(new Uri("Assets/Icons/positionMarkerPollen2.png", UriKind.Relative));
            //img.Margin = new Thickness(79, -10, 0, 0);

            //Grid grid = new Grid();
            //grid.Margin = new Thickness(10);
            //grid.Children.Add(border);
            //grid.Children.Add(cBar);
            //grid.Children.Add(img);


            


            //// Create an image for every Concentration for use in Background Task
            //    WriteableBitmap bmp = new WriteableBitmap(173, 173);
            //    Transform trans = new TranslateTransform();
            //    bmp.Render(rect, trans);
            //    bmp.Render(tbkName, trans);
            //    bmp.Render(grid, trans);
            //    bmp.Render(txbHeader, trans);
            //    bmp.Invalidate();


            //    Image h = new Image();
            //    h.Height = h.Width = 173;
            //    h.Source = bmp;

            //    spnHeader.Children.Clear();
            //    spnHeader.Children.Add(h);

        }

        private void Pollen_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel s = sender as StackPanel;

            if (s != null)
            {
                // Set currentPlace to selected place
                Pollen currentPollen = s.DataContext as Pollen;

                if (currentPollen != null)
                {
                    MainViewModel.Current.CurrentPollen = currentPollen; //MainViewModel.Current.AllPollen[MainViewModel.Current.CurrentPlace.PollenList.IndexOf(currentPollen)];

                    // Navigate to PollenDetails
                    NavigationService.Navigate(new Uri("/Views/PollenDetails.xaml", UriKind.Relative));
                }
            }
        }

        private void Pollen_TapTomorrow(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid s = sender as Grid;

            if (s != null)
            {
                // Set currentPlace to selected place
                Pollen currentPollen = s.DataContext as Pollen;

                if (currentPollen != null)
                {
                    MainViewModel.Current.CurrentPollen = currentPollen; // MainViewModel.Current.AllPollen[MainViewModel.Current.CurrentPlace.PollenListTomorrow.IndexOf(currentPollen)];

                    // Navigate to PollenDetails
                    NavigationService.Navigate(new Uri("/Views/PollenDetails.xaml", UriKind.Relative));
                }
            }
        }        

        private void Pollen_TapDayAfterTomorrow(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel s = sender as StackPanel;

            if (s != null)
            {
                // Set currentPlace to selected place
                Pollen currentPollen = s.DataContext as Pollen;

                if (currentPollen != null)
                {
                    MainViewModel.Current.CurrentPollen = currentPollen; // MainViewModel.Current.AllPollen[MainViewModel.Current.CurrentPlace.PollenListDayAfterTomorrow.IndexOf(currentPollen)];

                    // Navigate to PollenDetails
                    NavigationService.Navigate(new Uri("/Views/PollenDetails.xaml", UriKind.Relative));
                }
            }
        }

        private void appBarSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {        	            
            NavigationService.Navigate(new Uri("/Views/PlaceAddEdit.xaml?SaveFromSearch=true", UriKind.Relative));
            //NavigationService.RemoveBackEntry();
        }

        private void btnBuyTomorrow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	MarketplaceDetailTask marketplace = new MarketplaceDetailTask();
			marketplace.Show();
        }
         
    }
}