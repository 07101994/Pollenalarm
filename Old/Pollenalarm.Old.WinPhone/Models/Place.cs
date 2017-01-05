using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Net;
using HtmlAgilityPack;
using System.Windows;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Xna.Framework.GamerServices;
using System.Windows.Threading;
using Microsoft.Phone.Tasks;
//using ProTile.Lib;
using Pollenalarm.Old.WinPhone.Helper;
using Pollenalarm.Old.WinPhone.ViewModels;

namespace Pollenalarm.Old.WinPhone.Models
{
    public class Place : NotifyClass
    {
        public delegate void LoadingFinishedEventHander(object sender);
        public event LoadingFinishedEventHander LoadingFinished;

        // ----------------------------------------
        // Properties
        // ----------------------------------------

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { if (value != _ID) { _ID = value; NotifyPropertyChanged("ID"); } }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { if (value != _Name) { _Name = value; NotifyPropertyChanged("Name"); NotifyPropertyChanged("NameUpper"); } }
        }
        public string NameUpper
        {
            get { return Name.ToUpper(); }
        }    

        private string _Plz;
        public string Plz
        {
            get { return _Plz; }
            set { if (value != _Plz) { _Plz = value; NotifyPropertyChanged("Plz"); } }
        }

        private string _StrongestPollen = "keine Polle";
        public string StrongestPollen
        {
            get { return _StrongestPollen; }
            set { if (value != _StrongestPollen) { _StrongestPollen = value; NotifyPropertyChanged("StrongestPollen"); } }
        }

        public int AverageConcentrationWidth
        {
            get 
            {
                switch (AverageConcentrationInt)
                {
                    case 0: default:
                        return 0;
                    case 1:
                        return 40;
                    case 2:
                        return 80;
                    case 3:
                        return 120;
                }
            }            
        }

        private string _AverageConcentration;
        public string AverageConcentration
        {
            get { return _AverageConcentration; }
            set { if (value != _AverageConcentration) { _AverageConcentration = value; NotifyPropertyChanged("AverageConcentration"); NotifyPropertyChanged("AverageConcentrationInt"); NotifyPropertyChanged("AverageConcentrationWidth"); } }
        }

        private int _AverageConcentrationInt;
        public int AverageConcentrationInt
        {
            get { return _AverageConcentrationInt; }
            set { if (value != _AverageConcentrationInt) { _AverageConcentrationInt = value; NotifyPropertyChanged("AverageConcentrationInt"); } }
        }  

        

        private ObservableCollection<Pollen> _PollenList;
        public ObservableCollection<Pollen> PollenList
        {
            get 
            {
                if (_PollenList == null)
                    _PollenList = new ObservableCollection<Pollen>();                
                return _PollenList;
            }
            set { if (value != _PollenList) { _PollenList = value; NotifyPropertyChanged("PollenList"); } }
        }

        private ObservableCollection<Pollen> _PollenListTomorrow;
        public ObservableCollection<Pollen> PollenListTomorrow
        {
            get
            {
                if (_PollenListTomorrow == null)
                    _PollenListTomorrow = new ObservableCollection<Pollen>();
                return _PollenListTomorrow;
            }
            set { if (value != _PollenListTomorrow) { _PollenListTomorrow = value; NotifyPropertyChanged("PollenListTomorrow"); } }
        }

        private ObservableCollection<Pollen> _PollenListDayAfterTomorrow;
        public ObservableCollection<Pollen> PollenListDayAfterTomorrow
        {
            get
            {
                if (_PollenListDayAfterTomorrow == null)
                    _PollenListDayAfterTomorrow = new ObservableCollection<Pollen>();
                return _PollenListDayAfterTomorrow;
            }
            set { if (value != _PollenListDayAfterTomorrow) { _PollenListDayAfterTomorrow = value; NotifyPropertyChanged("PollenListDayAfterTomorrow"); } }
        }

        private bool _IsCurrentPosition = false;
        public bool IsCurrentPosition
        {
            get { return _IsCurrentPosition; }
            set { if (value != _IsCurrentPosition) { _IsCurrentPosition = value; NotifyPropertyChanged("IsCurrentPosition"); } }
        }       

        public bool IsContextMenuVisible
        {
            get
            {
                if (!IsCurrentPosition)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Visibility MenuVisibility
        {
            get
            {
                if (!IsCurrentPosition)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }


        // ----------------------------------------
        // Constructors
        // ----------------------------------------

        public Place() { }

        public Place(int id, string name, string plz)
        {
            this.ID = id;
            this.Name = name;
            this.Plz = plz;
            this.AverageConcentration = "";
        }


        // ----------------------------------------
        // Download and Parse Pollen Methods
        // ----------------------------------------
        
        // Download HTML
        public void DownloadPollenList()
        {
            // Start Progress Indicator on MainPage
            MainPage.ShellProgressStart("Polleninformationen werden aktualisiert.");

            // Download Pollendata from the mobile HEXAL website
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri("http://thepagedot.de/pollenalarm/pollen.php?do=getPollen&plz=" + Plz));
        }

        // Start HTML Parser
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                // Delete Current Lists
                if (PollenList != null)
                    PollenList.Clear();
                if (PollenListTomorrow != null)
                    PollenListTomorrow.Clear();
                if (PollenListDayAfterTomorrow != null)
                    PollenListDayAfterTomorrow.Clear();

                // Parse Downloaded Web-Result to get Pollen-Information
                ParseWebResult(HttpUtility.HtmlDecode(e.Result));
            }
            else
            {
                // Stop Progress Indicator on MainPage
                MainPage.ShellProgessStop();
                //MessageBox.Show("Beim Download der Daten für " + Name + " ist uns ein Fehler unterlaufen. Versuchen Sie es bitte erneut!", "Entschuldigung", MessageBoxButton.OK);
            }

            LoadingFinishedEventHander loadingFinishedEventHander = LoadingFinished;
            if (loadingFinishedEventHander != null)
            {
                loadingFinishedEventHander(this);
            }
        }

        private void ParseWebResult(string webResult)
        {
            XDocument data = XDocument.Parse(webResult);

            int dayCounter = 0;
            foreach (XElement xmlDay in data.Descendants("day"))
            {
                foreach (XElement xmlPollen in xmlDay.Descendants("pollen"))
                {
                    string tempPollenName = xmlPollen.Element("name").Value;
                    bool tempPollenIsSelected = MainViewModel.Current.AllPollen.Where(x => x.Name.Equals(tempPollenName)).First().IsSelected;

                    Pollen tempPollen = new Pollen(tempPollenName, xmlPollen.Element("concentration").Value, tempPollenIsSelected);

                    tempPollen.BloomTime = MainViewModel.Current.AllPollen.Where(x => x.Name.Equals(tempPollenName)).First().BloomTime;
                    tempPollen.GeneralPollenInformation = MainViewModel.Current.AllPollen.Where(x => x.Name.Equals(tempPollenName)).First().GeneralPollenInformation;
                    tempPollen.ImageCredits = MainViewModel.Current.AllPollen.Where(x => x.Name.Equals(tempPollenName)).First().ImageCredits;
                    tempPollen.ClinicalPollution = MainViewModel.Current.AllPollen.Where(x => x.Name.Equals(tempPollenName)).First().ClinicalPollution;

                    switch (dayCounter)
                    {
                        case 0: default:
                            PollenList.Add(tempPollen);
                            break;
                        case 1:
                            PollenListTomorrow.Add(tempPollen);
                            break;
                        case 2:
                            PollenListDayAfterTomorrow.Add(tempPollen);
                            break;
                    }
                }

                dayCounter++;
            }            

            // Calculate Max Concentration
            SetMaxConcentration();

            MainPage.ShellProgessStop();
            
            //MessageBox.Show("Finished!");
        }

        public void OfflineUpdate()
        {
            if (PollenList != null)
            {
                int max = 0, counter = 0;

                if (PollenList.Count != PollenListTomorrow.Count)
                {
                    MessageBox.Show("Es scheint Schwierigkeiten mit den Servern der HEXAL-AG zu geben. Wenn im Verlauf des Tages Unstimmigkeiten auftreten sollten entschuldigen Sie dies bitte. Sollten auch morgen noch Probleme auftreten wenden Sie sich bitte an den Support.", "Server Probleme", MessageBoxButton.OK);
                }

                //MessageBox.Show("Anzahl der Pollen in " + Name + ":\n" + PollenList.Count.ToString() + " - " + PollenListTomorrow.Count.ToString() + " - " + PollenListDayAfterTomorrow.Count.ToString() + "\nAlle Zahlen müssten 14 sein.");

                for (int i = 0; i < PollenList.Count; i++)
                {
                    // Set IsSelected State of all pollen Lists to the selected one in Pollen Selection
                        PollenList[i].IsSelected = MainViewModel.Current.AllPollen.ElementAt(i).IsSelected;

                        PollenListTomorrow[i].IsSelected = MainViewModel.Current.AllPollen.ElementAt(i).IsSelected;

                        PollenListDayAfterTomorrow[i].IsSelected = MainViewModel.Current.AllPollen.ElementAt(i).IsSelected;
                }

                // Calculate Maximum Pollen Concentration for today
                foreach (Pollen pollen in PollenList)
                {
                    if (MainViewModel.Current.AllPollen.ElementAt(counter).IsSelected == true)
                    {
                        if (Convert.ToInt32(pollen.Concentration) > max)
                        {
                            max = Convert.ToInt32(pollen.Concentration);
                        }
                    }

                    counter++;
                }

                SetMaxConcentration();

                Debug.WriteLine("Max Concentration: " + max);
            }
        }

        public void SetMaxConcentration()
        {
            int maxConcentration = 0;
            foreach (Pollen pollen in PollenList)
            {
                if (pollen.IsSelected)
                {
                    if (pollen.ConcentrationToInt32() > maxConcentration)
                    {
                        maxConcentration = pollen.ConcentrationToInt32();
                        StrongestPollen = pollen.Name;
                    }
                }
            }

            AverageConcentrationInt = maxConcentration;

            if (maxConcentration == 0)
                AverageConcentration = App.ConcentrationArray[0];
            else if (maxConcentration <= 1)
                AverageConcentration = App.ConcentrationArray[1];
            else if (maxConcentration <= 2)
                AverageConcentration = App.ConcentrationArray[2];
            else if (maxConcentration <= 3)
                AverageConcentration = App.ConcentrationArray[3];
        }

        public void PinToStart()
        {
            if (App.License.IsTrial() == false || App.IsPromo == true)
            {
                if (LowMemoryHelper.IsLowMemDevice)
                {
                    MessageBoxResult result = MessageBox.Show("Bitte beachten Sie, dass Ihr Gerät evt. über nicht genügend Speicher verfügt, um die Kachel im Hintergrund zu aktualisieren. Dies führt dazu, dass Kacheln nur aktualisiert werden, wenn die Anwendung gestartet wird.\n\nWählen Sie 'Ok', um den Ort dennoch auf die Startseite zu heften.", "Keine Aktualisierung möglich", MessageBoxButton.OKCancel);
                    if (result != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                // Look to see if the tile already exists and if so, don't try to create again.
                ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("plz=" + Plz));

                string backContent;
                if (StrongestPollen.Equals("Gräser")) { backContent = "Am stärksten blühen " + StrongestPollen + "."; }
                else { backContent = "Am stärksten blüht " + StrongestPollen + "."; }

                StandardTileData NewTileData = new StandardTileData();
                NewTileData.BackgroundImage = CreateTileBackground(this);
                NewTileData.Title = "";
                NewTileData.BackContent = backContent;
                NewTileData.BackTitle = "Stand: " + DateTime.Now.ToString("HH:mm") + " Uhr";

                // Create the tile if we didn't find it already exists.
                if (TileToFind == null)
                {
                    ShellTile.Create(new Uri("/MainPage.xaml?id=" + ID + "&plz=" + Plz, UriKind.Relative), NewTileData);
                }
                else
                {
                    UpdateLiveTile(false); //TODO: When does this happen? True or False?
                }
            }
            else
            {
                App.ShowBuyMessage("");
            }
        }        

        public void UpdateLiveTile(bool isNameChanged)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("id=" + ID));

            if (TileToFind != null)
            {
                string backContent;
                if (StrongestPollen.Equals("Gräser")) { backContent = "Am stärksten blühen " + StrongestPollen + "."; }
                else { backContent = "Am stärksten blüht " + StrongestPollen + "."; }

                StandardTileData tileData = new StandardTileData();
                tileData.Title = "";
                tileData.BackContent = backContent;
                tileData.BackTitle = "Stand: " + DateTime.Now.ToString("HH:mm") + " Uhr";

                //bool accentChanged = false;
                //if (App.Settings.Contains("AccentColor"))
                //{
                //    SolidColorBrush savedAccentColor = new SolidColorBrush((Color)App.Settings["AccentColor"]);                   

                //    if (savedAccentColor.Color != ((SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"]).Color)
                //    {
                //        accentChanged = true;
                //    }
                //}
                //else
                //{
                //    accentChanged = true;
                //}

                //App.SaveToSettings("AccentColor", ((SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"]).Color);


                if (isNameChanged)
                {
                    // Create new Images if Name or Accent Color has changed
                    //MessageBox.Show("Paint Tile for " + Name + " new. \nName Changed: " + isNameChanged + "\nAccent changed: " + accentChanged);
                    tileData.BackgroundImage = CreateTileBackground(this);
                }
                else
                {
                    tileData.BackgroundImage = new Uri("isostore:/Shared/ShellContent/" + "img" + ID + "-" + AverageConcentration + ".png", UriKind.Absolute);
                }

                TileToFind.Update(tileData);
            }            
        }

        public void DeleteTile()
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("id=" + ID));

            // Delete the tile
            if (TileToFind != null)
            {
                TileToFind.Delete();
            }
        }

        public Uri CreateTileBackground(Place place)
        {
            // Create Tile Background
            Rectangle rect = new Rectangle();
            rect.Height = 336;
            rect.Width = 336;
            //rect.Fill = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"]; // Don't fill for transparent background

            // Place Name
            TextBlock tbkName = new TextBlock();
            tbkName.Text = place.NameUpper;
            tbkName.FontFamily = new FontFamily("Segoe UI");
            tbkName.Foreground = new SolidColorBrush(Colors.White);
            tbkName.FontSize = 44;
            tbkName.Padding = new Thickness(13, 262, 7, 13);
            tbkName.FontWeight = FontWeights.SemiBold;

            // Concentration Header
            TextBlock txbHeader = new TextBlock();
            txbHeader.Text = "Belastung";
            txbHeader.FontFamily = new FontFamily("Segoe WP");
            txbHeader.Foreground = new SolidColorBrush(Colors.White);
            txbHeader.FontSize = 37;
            txbHeader.Padding = new Thickness(13, 143, 0, 0);

            // Border for Concentration Bar
            Rectangle border = new Rectangle();
            border.Height = 37;
            border.Width = 233;
            border.Stroke = new SolidColorBrush(Colors.White);
            border.StrokeThickness = 2;
            border.Margin = new Thickness(13, 204, 0, 0);            

            Image img = new Image();
            img.Source = new BitmapImage(new Uri("Assets/Icons/positionMarkerPollen2.png", UriKind.Relative));
            img.Margin = new Thickness(153, -19, 0, 0);

            Grid grid = new Grid();
            grid.Margin = new Thickness(19);
            grid.Children.Add(border);
            grid.Children.Add(img);      

            // Create an image for every Concentration for use in Background Task
            for (int i = 0; i < 4; i++)
            {
                // ConcentrationBar
                Rectangle cBar = new Rectangle();
                cBar.Height = 21;
                cBar.Width = 155; // 40 - 80 - 112
                cBar.Fill = new SolidColorBrush(Colors.White);
                cBar.StrokeThickness = 0;
                cBar.Margin = new Thickness(21, 212, 0, 0);

                switch (i)
                {
                    default:
                    case 0: cBar.Width = 0; break;
                    case 1: cBar.Width = 77; break;
                    case 2: cBar.Width = 155; break;
                    case 3: cBar.Width = 217; break;
                }

                grid.Children.Remove(cBar);
                grid.Children.Add(cBar);

                WriteableBitmap bmp = new WriteableBitmap(336, 336);
                Transform trans = new TranslateTransform();
                bmp.Render(rect, trans);
                bmp.Render(tbkName, trans);
                bmp.Render(grid, trans);
                bmp.Render(txbHeader, trans);
                bmp.Invalidate();

                string imageName = "img" + ID + "-" + App.ConcentrationArray[i]  + ".png";

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var isoFileStream = isoStore.CreateFile("/Shared/ShellContent/" + imageName))
                    {
                        //System.Windows.Media.Imaging.Extensions.SaveJpeg(bmp, isoFileStream, 336, 336, 0, 100);
                        bmp.SavePng(isoFileStream);
                    }
                }                
            }

            return new Uri("isostore:/Shared/ShellContent/" + "img" + ID + "-" + AverageConcentration + ".png", UriKind.Absolute);
        }




        //public Uri CreateTileBackgroundOld(Place place)
        //{
        //    // Create Tile Background
        //    Rectangle rect = new Rectangle();
        //    rect.Height = 173;
        //    rect.Width = 173;
        //    rect.Fill = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];

        //    // Concentration Header
        //    TextBlock txbHeader = new TextBlock();
        //    txbHeader.Text = "Belastung:";
        //    txbHeader.Foreground = new SolidColorBrush(Colors.White);
        //    txbHeader.FontSize = 22;
        //    txbHeader.Padding = new Thickness(7, 15, 0, 0);

        //    // Plz
        //    TextBlock txbPlz = new TextBlock();
        //    txbPlz.Text = Plz;
        //    txbPlz.Foreground = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255));
        //    //txbHeader.Foreground = new SolidColorBrush(Colors.White);
        //    txbPlz.FontSize = 72;
        //    txbPlz.Padding = new Thickness(52, 96, 0, 0);

        //    // Create an image for every Concentration for use in Background Task
        //    for (int i = 0; i < 4; i++)
        //    {
                               
        //        // Concentration
        //        TextBlock txbConcentration = new TextBlock();
        //        txbConcentration.Text = App.ConcentrationArray[i];
        //        txbConcentration.Foreground = new SolidColorBrush(Colors.White);
        //        txbConcentration.FontSize = 32;
        //        txbConcentration.Padding = new Thickness(7, 38, 0, 0);

        //        WriteableBitmap bmp = new WriteableBitmap(173, 173);
        //        Transform trans = new TranslateTransform();
        //        bmp.Render(rect, trans);
        //        bmp.Render(txbHeader, trans);
        //        bmp.Render(txbPlz, trans);
        //        bmp.Render(txbConcentration, trans);
        //        bmp.Invalidate();

        //        string imageName = "img" + ID + "-" + App.ConcentrationArray[i]  + ".jpg";

        //        using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            using (var isoFileStream = isoStore.CreateFile("/Shared/ShellContent/" + imageName))
        //            {
        //                System.Windows.Media.Imaging.Extensions.SaveJpeg(bmp, isoFileStream, 173, 173, 0, 100);
        //            }
        //        }                
        //    }

        //    return new Uri("isostore:/Shared/ShellContent/" + "img" + ID + "-" + AverageConcentration + ".jpg", UriKind.Absolute);
        //}
    }
}
