using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
//using HtmlAgilityPack;
using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

namespace Pollenalarm.Old.WinPhone.TileUpdaterBackgroundTask
{
    public class TempPlace
    {
        public string ID;
        public string Plz;
        public string AverageConcentration;
        public string StrongestPollen = "keine Polle";
        public ObservableCollection<TempPollen> PollenList;
        public ShellTile Tile;

        /// <summary>
        /// Download the HTML Code from the mobile Hexal Website
        /// </summary>
        public void DownloadPollenList()
        {
            // Download Pollendata from the mobile HEXAL website
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri("http://thepagedot.de/pollenalarm/pollen.php?do=getPollen&plz=" + Plz));
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Debug.WriteLine("Download für " + Plz + " gestartet.");

            if (e.Error == null && e.Cancelled == false)
            {
                // Start HTML Parser
                ParseWebResult(HttpUtility.HtmlDecode(e.Result));
            }
            else
            {
                Debug.WriteLine("Es ist ein Fehler beim Herunterladen der Polleninformation aufgetreten.");
            }
        }

        /// <summary>
        /// HTML Parser for mobile Hexal website
        /// </summary>
        /// <param name="webResult"></param>
        private void ParseWebResult(string webResult)
        {
            Debug.WriteLine("Parsing für " + Plz + " gestartet:");

            if (this.PollenList == null)
            {
                this.PollenList = new ObservableCollection<TempPollen>();
            }            

            XDocument data = XDocument.Parse(webResult);

            int counter = 0;
            foreach (XElement xmlPollen in data.Descendants("day").First().Descendants("pollen"))
            {

                string tempPollenName = xmlPollen.Element("name").Value;
                bool tempPollenIsSelected = ScheduledAgent.AllPollen.ElementAt(counter).IsSelected;

                TempPollen pollenToAdd = new TempPollen(tempPollenName, xmlPollen.Element("concentration").Value, tempPollenIsSelected);
                PollenList.Add(pollenToAdd);

                counter++;
            }
            
            // Calculate Average Concentration
            SetMaxConcentration();

            // Update Tile
            UpdateTile();
        }

        private void UpdateTile()
        {
            string backContent;
            if (StrongestPollen.Equals("Gräser")) { backContent = "Am stärksten blühen " + StrongestPollen + "."; }
            else { backContent = "Am stärksten blüht " + StrongestPollen + "."; }

            // Create Tile Data to update the Live Tile with
            StandardTileData tileData = new StandardTileData();
            tileData.BackgroundImage = GetTileBackground();
            tileData.BackTitle = "Stand: " + DateTime.Now.ToString("HH:mm") + " Uhr";
            tileData.BackContent = backContent;
            //tileData.Count = 1;
            Tile.Update(tileData);
            Debug.WriteLine("Ready");
        }

        /// <summary>
        /// Gets the according Tile-Image to the average concentration
        /// </summary>
        /// <returns></returns>
        private Uri GetTileBackground()
        {           
            string imageName = "img" + ID + "-" + AverageConcentration + ".png";
            Debug.WriteLine("Image Name:" + imageName);
            return new Uri("isostore:/Shared/ShellContent/" + imageName, UriKind.Absolute);
        }

        private void SetMaxConcentration()
        {
            int maxConcentration = 0;
            foreach (TempPollen pollen in PollenList)
            {
                if (pollen.IsSelected)
                {
                    if (Convert.ToInt32(pollen.Concentration) > maxConcentration)
                    {
                        maxConcentration = Convert.ToInt32(pollen.Concentration);
                        StrongestPollen = pollen.Name;
                    }
                }
            }

            if (maxConcentration == 0)
                AverageConcentration = "Keine";
            else if (maxConcentration <= 1)
                AverageConcentration = "Gering";
            else if (maxConcentration <= 2)
                AverageConcentration = "Mäßig";
            else if (maxConcentration <= 3)
                AverageConcentration = "Stark";

            Debug.WriteLine("AverageConcentration: " + AverageConcentration);
        }

    }

}
