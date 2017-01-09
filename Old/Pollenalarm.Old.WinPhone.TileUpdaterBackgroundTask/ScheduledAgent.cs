using System.Windows;
using Microsoft.Phone.Scheduler;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Diagnostics;
//using HtmlAgilityPack;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Device.Location;

namespace Pollenalarm.Old.WinPhone.TileUpdaterBackgroundTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        public static string BaseUri = "http://pollenalarm.azurewebsites.net/api/old";

        public static IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;
        private static volatile bool _classInitialized;
        public static ObservableCollection<TempPollen> AllPollen;
        private ShellTile currentTile;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            Debug.WriteLine("===== Background Task =====");

            //----------------------------------------
            // Check whether Task is allowed
            //----------------------------------------

            bool UpdateAlways = true;
            if (Settings.Contains("UpdateAlways"))
            {
                UpdateAlways = ((bool)Settings["UpdateAlways"]);
            }

            if (UpdateAlways == false)
            {
                DateTime UpdateFrom = new DateTime(), UpdateTo = new DateTime();
                bool ok1 = false, ok2 = false;
                if (Settings.Contains("UpdateSpanFrom")) { UpdateFrom = ((DateTime)Settings["UpdateSpanFrom"]); ok1 = true; }
                if (Settings.Contains("UpdateSpanTo")) { UpdateTo = ((DateTime)Settings["UpdateSpanTo"]); ok2 = true; }

                if (ok1 && ok2)
                {
                    DateTime TodayFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, UpdateFrom.Hour, UpdateFrom.Minute, 0);
                    DateTime TodayTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, UpdateTo.Hour, UpdateTo.Minute, 0);


                    //TODO: Check this:
                    // Update from is earlier than Update to
                    if (TodayFrom.CompareTo(TodayTo) > 0)
                    {
                        TodayTo = TodayTo.AddDays(1);
                        Debug.WriteLine("From is later.");
                    }

                    Debug.WriteLine("From: " + TodayFrom.ToString() + "\nNow:  " + DateTime.Now.ToString() + "\nTo:   " + TodayTo.ToString());


                    if (DateTime.Now.CompareTo(TodayFrom) > 0 && DateTime.Now.CompareTo(TodayTo) < 0)
                    {
                        Debug.WriteLine("Okay!");
                    }
                    else
                    {
                        Debug.WriteLine("Break!");
                        return;
                    }
                }
            }

            // Check last Background Task Time
            if (Settings.Contains("LastBackgroundTaskTime"))
            {
                int UpdateFrequence = 0;
                if (Settings.Contains("UpdateFrequence")) { UpdateFrequence = ((int)Settings["UpdateFrequence"]); }

                DateTime LastBackgroundTaskTime = ((DateTime)Settings["LastBackgroundTaskTime"]);
                DateTime compareTime = DateTime.Now;

                switch (UpdateFrequence)
                {
                    case 1: // 1 hour
                        LastBackgroundTaskTime = LastBackgroundTaskTime.AddHours(1);
                        break;
                    case 2: // 2 Hours
                        LastBackgroundTaskTime = LastBackgroundTaskTime.AddHours(2);
                        break;
                    case 3: // 6 Hours
                        LastBackgroundTaskTime = LastBackgroundTaskTime.AddHours(6);
                        break;
                    case 4: // 12 Hours
                        LastBackgroundTaskTime = LastBackgroundTaskTime.AddHours(12);
                        break;
                    default:
                        break;
                }

                Debug.WriteLine("Last Background Time + Wait Time: " + LastBackgroundTaskTime.ToString() + "\n Nowtime: " + compareTime);

                // Last Background Time (added with wait time) is later than compate time
                if (LastBackgroundTaskTime.CompareTo(compareTime) > 0)
                {
                    Debug.WriteLine("Break!");
                    return;
                }

            }
            else
            {
                Debug.WriteLine("WHOAR!");
            }


            //----------------------------------------
            // Main part of Background Task
            //----------------------------------------

            AllPollen = LoadPollenFromIsolatedStorage();

            foreach (TempPollen pollen in AllPollen)
            {
                Debug.WriteLine(pollen.Name + " - " + pollen.IsSelected);
            }

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                if (tile.NavigationUri.ToString().Contains("plz"))
                {
                    //TODO: Implement Current Position

                    // Set tile as currentTile
                    currentTile = tile;

                    // Get Plz out of NavigationUri
                    string[] parts = tile.NavigationUri.ToString().Split('?');
                    string[] parameters = parts[1].Split('&');

                    string plz = parameters[1].Substring(4);
                    Debug.WriteLine("Plz: " + parameters[1].Substring(4) + " - ID: " + parameters[0]);

                    TempPlace place = new TempPlace();
                    place.ID = parameters[0].Substring(3);
                    place.Tile = tile;
                    place.Plz = plz;
                    place.DownloadPollenList();
                }
            }

            // Save Current Time
            if (Settings.Contains("LastBackgroundTaskTime"))
            {
                Settings["LastBackgroundTaskTime"] = DateTime.Now;
                Debug.WriteLine("Time edited Saved!");
            }
            else
            {
                Settings.Add("LastBackgroundTaskTime", DateTime.Now);
                Debug.WriteLine("Time new Saved!");
            }

            Settings.Save();

            //TODO: Check whether Time Saved does not come on break!

            // TODO: Can I outcomment that?:
            //NotifyComplete();
        }

        public static ObservableCollection<TempPollen> LoadPollenFromIsolatedStorage()
        {
            // Filename
            string fileName = "savedPollen.xml";

            // Temporary list of Pollen
            ObservableCollection<TempPollen> pollenList = new ObservableCollection<TempPollen>();

            using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Check whether file Exists
                if (!isolatedStorage.FileExists(fileName))
                {
                    return pollenList;
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

                        var xmlPollen = from c in xmlPlaces.Descendants("pollen") select new TempPollen(c.Element("name").Value, "", Convert.ToBoolean(c.Element("isSelected").Value));

                        // Add Places to list of Pollen
                        foreach (TempPollen pollen in xmlPollen)
                        {
                            pollenList.Add(pollen);
                        }

                        return pollenList;
                    }
                    catch (XmlException)
                    {
                        //MessageBox.Show("Es ist ein Fehler in Ihren gespeicherten Daten aufgetreten. Ihre Daten wurden gelöscht. Sollte dieses Problem häufiger auftreten wenden Sie sich bitte an den Support!\n", "Entschuldigung", MessageBoxButton.OK);

                        return new ObservableCollection<TempPollen>();
                    }
                }
            }
        }
    }
}