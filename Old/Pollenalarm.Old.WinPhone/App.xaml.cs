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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using System.Device.Location;
using Microsoft.Phone.Marketplace;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Phone.Tasks;
using Pollenalarm.Old.WinPhone.Models;
using Pollenalarm.Old.WinPhone.ViewModels;

namespace Pollenalarm.Old.WinPhone
{
    public partial class App : Application
    {
        public static string BaseUri = "http://pollenalarm.azurewebsites.net/api/old";

        public static Place CurrentPlace;
        public static Place CurrentPositionPlace;
        public static string[] ConcentrationArray = { "Keine", "Gering", "Mäßig", "Stark" };

        public static bool IsGeneralInformationLoaded = false;
        public static bool ShowAllPollen = true;
        public static bool hideTrialsOnFullVersion = true; // Muss true sein
        public static bool IsAccerlermoeterSupported = true;
        public static bool IsPromo = false;

        public static LicenseInformation License;

        public static IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        public void SavePlacesToIsolatedStorage()
        {
            try
            {
                using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // Filename
                    string fileName = "savedPlaces.xml";

                    // Delete file if it already exists
                    if (isolatedStorage.FileExists(fileName))
                    {
                        isolatedStorage.DeleteFile(fileName);
                    }

                    // Open file
                    using (StreamWriter streamWriter = new StreamWriter(isolatedStorage.OpenFile(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        streamWriter.WriteLine("<places>");
                        streamWriter.WriteLine("</places>");
                        streamWriter.Close();
                    }

                    // Get XML out of file
                    StreamReader streamReader = new StreamReader(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read));
                    XDocument xmlPlaces = XDocument.Parse(streamReader.ReadToEnd());
                    streamReader.Close();

                    // Add Places to XML
                    XElement rootNode = xmlPlaces.Root;
                    foreach (Place place in MainViewModel.Current.AllPlaces)
                    {
                        if (place.IsCurrentPosition == false)
                        {
                            XElement xmlPlace = new XElement("place", new XElement("id", place.ID), new XElement("name", place.Name), new XElement("plz", place.Plz));
                            rootNode.Add(xmlPlace);
                        }
                    }

                    // Save XML to 
                    using (StreamWriter streamWriter = new StreamWriter(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Write)))
                    {
                        streamWriter.Write(xmlPlaces.ToString());
                        streamWriter.Close();
                    }

                    //MessageBox.Show("Dateien gespeichert.\n" + xmlPlaces.ToString());                    
                }
            }
            catch (IsolatedStorageException)
            {
                //TODO: Fix error! Everytime you press BACK without changing anything on the MainPage an IsolatedStorageException occurs!
                Debug.WriteLine("ERROR 001: Nothing was Saved, No changings were made.");
                MessageBox.Show("ERROR 001: Nothing was Saved, No changings were made.");
            }
        }

        public static ObservableCollection<Place> LoadPlacesFromIsolatedStorage()
        {
            // Filename
            string fileName = "savedPlaces.xml";

            // Temporary list of Pollen
            ObservableCollection<Place> placesList = new ObservableCollection<Place>();

            using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Check whether file Exists
                if (!isolatedStorage.FileExists(fileName))
                {
                    return placesList;
                }
                else
                {
                    try
                    {
                        // Get XML Places out of File
                        StreamReader streamReader = new StreamReader(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read));
                        XDocument xmlPlaces = XDocument.Parse(streamReader.ReadToEnd());
                        streamReader.Close();

                        //XDocument xmlPlaces = XDocument.Load(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read));                        

                        var places = from c in xmlPlaces.Descendants("place") select new Place(Convert.ToInt32(c.Element("id").Value), c.Element("name").Value, (c.Element("plz").Value));

                        // Add Places to list of Pollen
                        foreach (Place place in places)
                        {
                            placesList.Add(place);
                        }

                        return placesList;
                    }
                    catch (XmlException)
                    {
                        //MessageBox.Show("Es ist ein Fehler in Ihren gespeicherten Daten aufgetreten. Ihre Daten wurden gelöscht. Sollte dieses Problem häufiger auftreten wenden Sie sich bitte an den Support!\n", "Entschuldigung", MessageBoxButton.OK);

                        return new ObservableCollection<Place>();
                    }
                    catch (NullReferenceException)
                    {
                        isolatedStorage.DeleteFile(fileName);
                        return new ObservableCollection<Place>();
                    }
                }
            }
        }

        public void SavePollenToIsolatedStorage()
        {
            try
            {
                using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // Filename
                    string fileName = "savedPollen.xml";

                    // Delete file if it already exists
                    if (isolatedStorage.FileExists(fileName))
                    {
                        isolatedStorage.DeleteFile(fileName);
                    }

                    // Open file
                    using (StreamWriter streamWriter = new StreamWriter(isolatedStorage.OpenFile(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        streamWriter.WriteLine("<allpollen>");
                        streamWriter.WriteLine("</allpollen>");
                        streamWriter.Close();
                    }

                    // Get XML out of file
                    StreamReader streamReader = new StreamReader(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read));
                    XDocument xmlPlaces = XDocument.Parse(streamReader.ReadToEnd());
                    streamReader.Close();

                    // Add Places to XML
                    XElement rootNode = xmlPlaces.Root;
                    foreach (Pollen pollen in MainViewModel.Current.AllPollen)
                    {

                        XElement xmlPlace = new XElement("pollen", new XElement("name", pollen.Name), new XElement("isSelected", pollen.IsSelected));
                        rootNode.Add(xmlPlace);

                    }

                    // Save XML to 
                    using (StreamWriter streamWriter = new StreamWriter(isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Write)))
                    {
                        streamWriter.Write(xmlPlaces.ToString());
                        streamWriter.Close();
                    }

                    //MessageBox.Show("Dateien gespeichert.\n" + xmlPlaces.ToString());                    
                }
            }
            catch (IsolatedStorageException)
            {
                //TODO: Fix error! Everytime you press BACK without changing anything on the MainPage an IsolatedStorageException occurs!
                Debug.WriteLine("ERROR 002: Nothing was Saved, No changings were made.");
                MessageBox.Show("ERROR 002: Nothing was Saved, No changings were made.");
            }
        }

        public static string AsciName(string name)
        {
            name = name.Replace("ß", "ss");
            name = name.Replace("ä", "ae");
            return name;
        }

        public static void SaveToSettings(string key, object value)
        {
            if (Settings.Contains(key))
            {
                Settings[key] = value;
            }
            else
            {
                Settings.Add(key, value);
            }

            Settings.Save();
        }

        /// <summary>
        /// Load objects from Isolated Storage Settings
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Return null if Key doesn't exist</returns>
        public static object LoadFromSettings(string key, object returnIfNotFound)
        {
            if (Settings.Contains(key))
            {
                return Settings[key];
            }
            else
            {
                return returnIfNotFound;
            }
        }

        public static bool CheckSettings(string key)
        {
            if (App.Settings.Contains(key))
            {
                if (((bool)App.Settings[key]) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateAllLiveTiles()
        {
            foreach (Place place in MainViewModel.Current.AllPlaces)
            {
                place.UpdateLiveTile(false); // TODO Check
            }
        }

        public void LoadSettings()
        {
            if (App.Settings.Contains("ShowAllPollen"))
            {
                if (((bool)App.Settings["ShowAllPollen"]) == false)
                {
                    ShowAllPollen = false;
                }
            }

            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.Start();

            if (watcher.Permission == GeoPositionPermission.Denied)
            {
                SaveToSettings("LocationServicesAllowed", false);
            }
        }

        public static int GetNextPlaceID()
        {
            if (Settings.Contains("CurrentPlaceID"))
            {
                int currentPlaceID = ((int)App.Settings["CurrentPlaceID"]);
                SaveToSettings("CurrentPlaceID", currentPlaceID + 1);
                return currentPlaceID + 1;
            }
            else
            {
                int currentPlaceID = 100;
                SaveToSettings("CurrentPlaceID", currentPlaceID);
                return currentPlaceID++;
            }
        }

        public static void ShowBuyMessage(string s)
        {
            string message;

            if (s.Length == 0)
            {
                message = "Diese Funktion ist in der Testversion leider nicht verfügbar. Kaufen Sie die Vollversion, um den vollen Funktionsumfang nutzen zu können.";
            }
            else
            {
                message = s;
            }

            List<string> messageBoxButtons = new List<string>();
            messageBoxButtons.Add("Marketplace");
            messageBoxButtons.Add("Abbrechen");
            IAsyncResult result = Guide.BeginShowMessageBox("Vollversion kaufen", message, messageBoxButtons, 0, MessageBoxIcon.Alert, new AsyncCallback(OnBuyMessageClosed), null);
        }

        private static void OnBuyMessageClosed(IAsyncResult ar)
        {
            int? buttonIndex = Guide.EndShowMessageBox(ar);
            switch (buttonIndex)
            {
                case 0:
                    MarketplaceDetailTask marketplace = new MarketplaceDetailTask();
                    marketplace.Show();
                    break;
                default:
                    break;
            }
        }

        private void CheckPromo()
        {
            if (Settings.Contains("promo"))
            {
                if ((bool)Settings["promo"] == true)
                {
                    IsPromo = true;
                }
                else
                {
                    IsPromo = false;
                }
            }
            else
            {
                IsPromo = false;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //TiltEffect.SetIsTiltEnabled(RootFrame, true);
            License = new LicenseInformation();
            CheckPromo();
            LoadSettings();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //TiltEffect.SetIsTiltEnabled(RootFrame, true);
            License = new LicenseInformation();
            CheckPromo();
            LoadSettings();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SavePlacesToIsolatedStorage();
            SavePollenToIsolatedStorage();
            UpdateAllLiveTiles();
            SaveToSettings("AllPollen", MainViewModel.Current.AllPollen);
            //Settings.Save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SavePlacesToIsolatedStorage();
            SavePollenToIsolatedStorage();
            UpdateAllLiveTiles();
            SaveToSettings("AllPollen", MainViewModel.Current.AllPollen);
            //Settings.Save();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                MessageBox.Show("Uns scheint ein schwerwiegender Fehler unterlaufen zu sein und die App muss beendet werden.\n\nSollte diese Problem regelmäßig auftreten kontaktieren Sie bitte den Support. Vielen Dank für Ihr Verständnis.", "Entschuldigung", MessageBoxButton.OK);
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                MessageBox.Show("Uns scheint ein schwerwiegender Fehler unterlaufen zu sein und die App muss beendet werden.\n\nSollte diese Problem regelmäßig auftreten kontaktieren Sie bitte den Support. Vielen Dank für Ihr Verständnis.", "Entschuldigung", MessageBoxButton.OK);
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.

            //RootFrame = new PhoneApplicationFrame();
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion


    }
}