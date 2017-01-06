using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Device.Location;
//using Pollenalarm.MyBingMaps;
using System.Windows;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Phone.Net.NetworkInformation;
using System.Xml.Linq;
using Pollenalarm.Old.WinPhone.Models;
using Pollenalarm.Old.WinPhone.Helper;
using Microsoft.Phone.Maps.Services;

namespace Pollenalarm.Old.WinPhone.ViewModels
{
    public class MainViewModel : NotifyClass
    {
        public delegate void DownloadedCompletedHandler(object sender, EventArgs e);

        public MainViewModel()
        {
            _Current = this;
        }

        private static MainViewModel _Current;
        public static MainViewModel Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new MainViewModel();
                }
                return _Current;
            }
        }

        private Place _CurrentPlace;
        public Place CurrentPlace
        {
            get { return _CurrentPlace; }
            set { if (value != _CurrentPlace) { _CurrentPlace = value; NotifyPropertyChanged("CurrentPlace"); } }
        }

        private Pollen _CurrentPollen;
        public Pollen CurrentPollen
        {
            get { return _CurrentPollen; }
            set { if (value != _CurrentPollen) { _CurrentPollen = value; NotifyPropertyChanged("CurrentPollen"); } }
        }

        // List of all Pollen
        private ObservableCollection<Pollen> _AllPollen;
        public ObservableCollection<Pollen> AllPollen
        {
            get
            {
                if (_AllPollen == null)
                {
                    _AllPollen = new ObservableCollection<Pollen>();
                }
                return _AllPollen;
            }
            set { if (value != _AllPollen) { _AllPollen = value; NotifyPropertyChanged("AllPollen"); } }
        }

        // List of User Places
        private ObservableCollection<Place> _AllPlaces;
        public ObservableCollection<Place> AllPlaces
        {
            get
            {
                if (_AllPlaces == null)
                {
                    _AllPlaces = new ObservableCollection<Place>();
                }
                return _AllPlaces;
            }
            set { if (value != _AllPlaces) { _AllPlaces = value; NotifyPropertyChanged("AllPlaces"); } }
        }

        // Search Result Collections
        private ObservableCollection<Place> _SearchResultPlaces;
        public ObservableCollection<Place> SearchResultPlaces
        {
            get
            {
                if (_SearchResultPlaces == null)
                {
                    _SearchResultPlaces = new ObservableCollection<Place>();
                }
                return _SearchResultPlaces;
            }
            set { if (value != _SearchResultPlaces) { _SearchResultPlaces = value; NotifyPropertyChanged("SearchResultPlaces"); } }
        }

        private ObservableCollection<Pollen> _SearchResultPollen;
        public ObservableCollection<Pollen> SearchResultPollen
        {
            get
            {
                if (_SearchResultPollen == null)
                {
                    _SearchResultPollen = new ObservableCollection<Pollen>();
                }
                return _SearchResultPollen;
            }
            set { if (value != _SearchResultPollen) { _SearchResultPollen = value; NotifyPropertyChanged("SearchResultPollen"); } }
        }

        // General Polleninformation
        private string _GeneralInformation;
        public string GeneralInformation
        {
            get { return _GeneralInformation; }
            set { if (value != _GeneralInformation) { _GeneralInformation = value; NotifyPropertyChanged("GeneralInformation"); NotifyPropertyChanged("GeneralInformationShort"); } }
        }

        public string GeneralInformationShort
        {
            get
            {
                if (_GeneralInformation != null)
                {
                    return generalInformationShorter(_GeneralInformation);
                }
                else
                {
                    return _GeneralInformation;
                }
            }
        }
        private string generalInformationShorter(string info)
        {
            string shortenedInfos;
            shortenedInfos = info.Split('.')[0] + ".";

            if (shortenedInfos.Length > 80)
            {
                return shortenedInfos.Substring(0, 80) + "...";
            }

            return shortenedInfos;
        }

        public MapConcentration _GermanyConcentration;
        public MapConcentration GermanyConcentration
        {
            get { return _GermanyConcentration; }
            set { if (value != _GermanyConcentration) { _GermanyConcentration = value; NotifyPropertyChanged("GermanyConcentration"); } }
        }

        /// <summary>
        /// Loads Data
        /// </summary>
        public void LoadData()
        {
            // ----------------------------------------
            // Initialize Variables
            // ----------------------------------------

            // General Information
            GeneralInformation = (string)App.LoadFromSettings("GeneralInformation", "");

            // All Pollen
            if (App.Settings.Contains("AllPollen"))
            {
                Debug.WriteLine("AllPollen found.");
                AllPollen = ((ObservableCollection<Pollen>)App.Settings["AllPollen"]);

                if (AllPollen.Count < 14)
                {
                    Debug.WriteLine("AllPollen was corrupt. New ones created");
                    CreatePollen();
                }
            }
            else
            {
                //TODO: In Datenbank umwandeln!
                Debug.WriteLine("AllPollen not found. New Pollen List created.");
                CreatePollen();
            }

            // Get List of all places from Isolated Storage
            AllPlaces = App.LoadPlacesFromIsolatedStorage();

            //Add currentPosition
            if (App.CheckSettings("LocationServicesAllowed"))
            {
                AddCurrentPosition();
            }

            Refresh();
        }

        // --------------------------------------------------
        // Download Methods for Map Data
        // --------------------------------------------------

        private void GetMapData()
        {
            // Create new MapConcentration for Germany Map
            if (GermanyConcentration == null)
            {
                GermanyConcentration = new MapConcentration();
            }

            // Download Inforamtion for Germany Map
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(mapDownload_DownloadStringCompleted);
            //webClient.DownloadStringAsync(new Uri("http://thepagedot.de/pollenalarm/pollen.php?do=getGermanyConcentration"));
            webClient.DownloadStringAsync(new Uri(App.BaseUri + "/GetMapConcentration"));
        }

        // Start HTML Parser
        private void mapDownload_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                ParseWebResultForMap(HttpUtility.HtmlDecode(e.Result));
            }
            else
            {
                //MessageBox.Show("Es ist ein Fehler beim Herunterladen der Polleninformation für die Deutschlandkarte aufgetreten. Bitte überprüfen Sie Ihre Internetverbindung und versuchen Sie es erneut.", "Download fehlgeschlagen", MessageBoxButton.OK);
                MapDownloadCompleted(this, null);
                //TODO: EventArgs show that failed.
            }
        }

        public static event DownloadedCompletedHandler MapDownloadCompleted;
        private void ParseWebResultForMap(string webResult)
        {
            try
            {
                int counter = 0;
                XDocument data = XDocument.Parse(webResult);
                foreach (XElement xmlMapValue in data.Descendants("ort"))
                {
                    GermanyConcentration.setValue(counter, xmlMapValue.Element("belastung").Value);
                    counter++;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Map Concentration parsing failed.");
            }

            // Download Finished
            MapDownloadCompleted(this, null);
        }

        // --------------------------------------------------
        // End of Download Methods for Map Data
        // --------------------------------------------------



        private void CreatePollen()
        {
            AllPollen.Add(new Pollen() { Id = 1, Name = "Ambrosia", IsSelected = true, ClinicalPollution = ClinicalPollution.VeryHigh, BloomTime = "Ende Juni bis Ende Oktober", GeneralPollenInformation = "Die ursprünglich in Nordamerika heimische Pflanze wurde durch den internationalen Handel nach Europa verschleppt. Von einer einzelnen Pflanze können eine Milliarde Pollenkörner freigesetzt werden. Das Unkraut ist eine Rudelpflanze und fühlt sich in feuchteren Gebieten am wohlsten. Kreuzreaktionen mit anderen Korbblütlern wie Kamille, Sonnenblume, Margerite, Arnika, Gänseblümchen oder Gräsern sind nicht unüblich.", ImageCredits = "Erika Hartmann / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 2, Name = "Ampfer", IsSelected = true, ClinicalPollution = ClinicalPollution.Middle, BloomTime = "Ende April bis September", GeneralPollenInformation = "Es gibt ungefähr 130 verschiedene Ampferarten, die sich jedoch alle in feuchten Wiesen, Äckern und Gräben wohl fühlen. Ihre rötlich gefärbten Blätter sind das markanteste Erkennungsmerkmal. Kreuzreaktionen sind ehr unüblich.", ImageCredits = "Susanne Schmich / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 3, Name = "Beifuß", IsSelected = true, ClinicalPollution = ClinicalPollution.High, BloomTime = "Mitte Mai bis Ende Oktober", GeneralPollenInformation = "Beifuß wächst mit Vorliebe auf ungepflegten Flächen, an Wegrändern und an Ufern. Die ursprüngliche Verbreitung des Beifuß ist heute nicht mehr zu bestimmen, nachdem er durch den Menschen über fast alle nördlichen Gebiete der Erde verbreitet wurde. Es treten mit praktisch allen anderen Kornblütern Kreuzreaktionne auf. Teilweise auch mit rohem Sellerie,", ImageCredits = "knipseline / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 4, Name = "Birke", IsSelected = true, ClinicalPollution = ClinicalPollution.VeryHigh, BloomTime = "Februar bis Juli, ab 15 °C", GeneralPollenInformation = "Die Büten der Birke gelten zu den häufigsten Verursachern von Allergien. Augentränen, Niesreiz und Fließschnupfen sind typische Reaktionen. Kreuzreaktionen können mit Erle, Hasel und Buche auftreten.", ImageCredits = "Regina Kaute / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 5, Name = "Buche", IsSelected = true, ClinicalPollution = ClinicalPollution.Low, BloomTime = "Anfang März bis Ende Juni", GeneralPollenInformation = "Buchen kommen in Wälder mit reichlich Wasserangebot vor. Sie toleriert auch Schatten, was sie in nahezu jedem deutschen Wald beheimatet. Häufige Kreuzreaktionen treten mit Frühblühern auf.", ImageCredits = "Ilka Funke-Wellstein / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 6, Name = "Eiche", IsSelected = true, ClinicalPollution = ClinicalPollution.Middle, BloomTime = "Mitte April bis Ende Mai", GeneralPollenInformation = "Obwohl Eichenpollen nicht zu den Haupt-Allergiepollen zählen, ist eine allergische Reaktion auf diese recht verbreitet. Eichenpollen-Allergiker sind häufig auch gegen Birke und Buche allergisch.", ImageCredits = "Helene13 / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 7, Name = "Erle", IsSelected = true, ClinicalPollution = ClinicalPollution.High, BloomTime = "Ende Februar bis Ende März", GeneralPollenInformation = "Erlen wachsen gerne in Wassernähe, an Bachläufen, Ufern von Seen und Flüssen. Die Blätter der Erle bieten zahlreichen Falter- und Schmetterlingsarten Lebensraum und Nahrung. Erlenblüten weisen starke Kreuzreaktionen mit Birkenpollen auf.", ImageCredits = "Andreas Hermsdorf / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 8, Name = "Gräser", IsSelected = true, ClinicalPollution = ClinicalPollution.VeryHigh, BloomTime = "Mitte Mai bis Anfang August", GeneralPollenInformation = "Pollen-Allergene sind in allen Pflanzenteilen enthalten. So entstehen beim Schneiden der Halme und Blätter winzige Aerosoltropfen, die bei Inhalation genauso wie die Pollen allergische Reaktionen verursachen. Da die Gräser untereinander eng verwand sind, treten Kreuzreaktionen bei nahezu allen Gräsern auf.", ImageCredits = "Rosel Eckstein / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 9, Name = "Hasel", IsSelected = true, ClinicalPollution = ClinicalPollution.High, BloomTime = "Mitte Februar bis Ende März", GeneralPollenInformation = "Haseln sind nach Birkenpollen der zweithäufigster Allergieauslöser unter den Baumpollen. Sie kommen in Laubwäldern, an Waldrändern oder als Hecken vor und weisen eine starke Kreuzreaktion mit Birkenpollen, aber auch zur Erle und Buche auf.", ImageCredits = "Maja Dumat / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 10, Name = "Pappel", IsSelected = true, ClinicalPollution = ClinicalPollution.Low, BloomTime = "Mitte März bis Mitte April", GeneralPollenInformation = "Pappeln kommen an Flussufern und in Wäldern vor. Viele Arten wie die Schwarzpappel sind gegen Überflutung und auch Überschlickung tolerant, während Trockenheit oft schlecht vertragen wird. Kreuzreaktionen treten bei Nüssen und Obstbäumen auf.", ImageCredits = "Susanne Schmich / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 11, Name = "Roggen", IsSelected = true, ClinicalPollution = ClinicalPollution.VeryHigh, BloomTime = "Mitte April bis Ende Juli", GeneralPollenInformation = "Es gibt Sommer- und Winterroggen, wobei in Mitteleuropa fast ausschließlich Winterroggen angebaut wird. Winterroggen ist die winterhärteste Getreideart, die Wintertemperaturen bis −25 ° C übersteht. Kreuzreaktionen treten vorallem mit anderen Gräsern auf.", ImageCredits = "s.media / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 12, Name = "Ulme", IsSelected = true, ClinicalPollution = ClinicalPollution.Low, BloomTime = "Anfang März bis Ende April", GeneralPollenInformation = "Die Ulme kommt in Wälder und Gebüschen vor und sind vom sogenannten Ulmensterben bedroht, das die mitteleuropäischen Ulmen-Arten ausrotten könnte. Es sind keine Kreuzreaktionen bekannt.", ImageCredits = "Elke Sawistowski / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 13, Name = "Wegerich", IsSelected = true, ClinicalPollution = ClinicalPollution.Middle, BloomTime = "Ende Mai bis Ende August", GeneralPollenInformation = "Es gibt viele verschiedene Arten des Wegerichs. Die bekanntesten sind Spitz- und Breitwegerich. Wegerich kommt am Wegrand und auf wiesen vor und weist Kreuzreaktionen mit Sellerie, Knoblauch, Karotten, Paprika, Anis, Curry und Kümmel auf.", ImageCredits = "Luise / pixelio.de" });
            AllPollen.Add(new Pollen() { Id = 14, Name = "Weide", IsSelected = true, ClinicalPollution = ClinicalPollution.Low, BloomTime = "Mitte März bis Anfang Mai", GeneralPollenInformation = "Weide kommt bevorzugt an feuchten Standorten vor. Unter den Weidenarten gibt es bis 30 Meter hohe Bäume, aber auch Zwergsträucher, die nur 3 Zentimeter hoch werden. Die baumartig wachsenden Weidenarten sind in der Regel schnellwüchsig, aber auch relativ kurzlebig. Es wurden Kreuzreaktionen mit Pappelpollen festgestellt.", ImageCredits = "Susanne Schmich  / pixelio.de" });
        }

        public void Refresh()
        {


            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Download Genereral PollenInformation
                DownloadGenrealInformation();

                // Download Map Data
                GetMapData();


                // Download Pollen Data for each Place an Add it to AllPollen List
                foreach (Place place in AllPlaces)
                {
                    if (place.IsCurrentPosition == false)
                    {
                        place.DownloadPollenList();
                    }
                }
            }
            else
            {
                MessageBox.Show("Leider verfügen Sie über keine ausreichende Netzwerkverbindung, sodass keine Pollendaten abgefragt werden können.\nBitte verbinden Sie Ihr Gerät mit dem Internet und versuchen Sie es erneut.", "Keine Netzwerkverbindung", MessageBoxButton.OK);
                MainPage.ShellProgessStop();
                MainPage.mapLoaded = true;
            }
        }

        public void AddCurrentPosition()
        {

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Check wether current postiion is already added
                if (App.CurrentPositionPlace == null)
                {
                    App.CurrentPositionPlace = new Place();
                    App.CurrentPositionPlace.Name = "Standort";
                    App.CurrentPositionPlace.ID = App.GetNextPlaceID();
                    App.CurrentPositionPlace.IsCurrentPosition = true;

                    AllPlaces.Insert(0, App.CurrentPositionPlace);
                    CreatePlaceForCurrentPosition();
                }
            }
            //else
            //{
            //    MessageBox.Show("Die aktuelle Position kann ohne Internetverbindung nicht bestimmt werden.", "Ortung fehlgeschlagen", MessageBoxButton.OK);
            //}
        }

        public void RemoveCurrentPosition()
        {
            AllPlaces.Remove(App.CurrentPositionPlace);
            App.CurrentPositionPlace = null;
        }

        private GeoCoordinateWatcher watcher;
        private void CreatePlaceForCurrentPosition()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); // using high accuracy
                watcher.MovementThreshold = 20; // use MovementThreshold to ignore noise in the signa
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                watcher.Start();
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (watcher != null)
            {
                try
                {
                    watcher.Stop();
                    watcher.Dispose();
                    watcher = null;                  

                    ReverseGeocodeQuery reverseGeocodeQuery = new ReverseGeocodeQuery();
                    reverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
                    reverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
                    reverseGeocodeQuery.QueryAsync();

                    //// Set the credentials using a valid Bing Maps key
                    //reverseGeocodeRequest.Credentials = new MyBingMaps.Credentials();
                    //reverseGeocodeRequest.Credentials.ApplicationId = "Aq69-RXbmx-nbDdlZISg__ioXRQH29f2-jgseV9r1acfqU6jFyCWsPfjANiy4o76";
                    //// Old Key: Al4TvQ0Vq0uH94ML8DXdrsGADDrc_ez5fSOpBJzBy2-bWboE30DGu165Y32-R9hT

                    //MyBingMaps.Location point = new MyBingMaps.Location();
                    //point.Latitude = e.Position.Location.Latitude;
                    //point.Longitude = e.Position.Location.Longitude;

                    //reverseGeocodeRequest.Location = point;

                    //// Make the reverse geocode request
                    //GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                    //geocodeService.ReverseGeocodeCompleted += new EventHandler<ReverseGeocodeCompletedEventArgs>(geocodeService_ReverseGeocodeCompleted);
                    //geocodeService.ReverseGeocodeAsync(reverseGeocodeRequest);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    RemoveCurrentPosition();
                }
            }
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            try
            {                            
                //((Place)AllPlaces.ElementAt(0)).Name = e.Result.Results[0].Address.Locality;
                ((Place)AllPlaces.ElementAt(0)).Plz = e.Result[0].Information.Address.PostalCode;
                ((Place)AllPlaces.ElementAt(0)).DownloadPollenList();
            }
            catch (Exception)
            {
                RemoveCurrentPosition();
            }
        }

        public void OfflineUpdateAllPlaces()
        {
            foreach (Place place in AllPlaces)
            {
                place.OfflineUpdate();
            }
        }

        // Download HTML for General Information
        public void DownloadGenrealInformation()
        {
            // Start Progress Indicator on MainPage
            //MainPage.ShellProgressStart("Informationen werden aktualisiert.");

            // Download Pollendata from the mobile HEXAL website
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            //webClient.DownloadStringAsync(new Uri("http://thepagedot.de/pollenalarm/pollen.php?do=getInformation"));
            webClient.DownloadStringAsync(new Uri(App.BaseUri + "/GetGeneralInformation"));
        }

        // Start HTML Parser
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                ParseWebResultForInformation(HttpUtility.HtmlDecode(e.Result));
            }
            else
            {
                // Stop Progress Indicator on MainPage
                MainPage.ShellProgessStop();
                MessageBox.Show("Es ist ein Fehler beim Herunterladen der Polleninformation aufgetreten. Bitte überprüfen Sie Ihre Internetverbindung und versuchen Sie es erneut.", "Download fehlgeschlagen", MessageBoxButton.OK);
            }
        }

        // TODO: Cobine Funktion with the one from Place
        private void ParseWebResultForInformation(string webResult)
        {
            try
            {
                XDocument data = XDocument.Parse(webResult);
                foreach (XElement xmlInformation in data.Descendants("information"))
                {
                    MainViewModel.Current.GeneralInformation = xmlInformation.Value;
                    App.SaveToSettings("GeneralInformation", xmlInformation.Value);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Parsing General Information failed.");
            }

            MainPage.ShellProgessStop();
        }
    }
}
