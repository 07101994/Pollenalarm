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

namespace Pollenalarm.Old.WinPhone.TileUpdaterBackgroundTask
{
    public class TempPollen
    {
        public string Name;
        public bool IsSelected;
        public Concentration Concentration;
        public string ConcentrationString;

        public TempPollen(string name, string concentrationString, bool isSelected)
        {
            this.Name = name;
            this.IsSelected = isSelected;

            switch (concentrationString)
            {
                case "-":
                    this.Concentration = Concentration.None;
                    this.ConcentrationString = "Keine";
                    break;
                case "schwach":
                    this.Concentration = Concentration.Low;
                    this.ConcentrationString = "Schwach";
                    break;
                case "mäßig":
                    this.Concentration = Concentration.Middle;
                    this.ConcentrationString = "Mäßig";
                    break;
                case "stark":
                    this.Concentration = Concentration.High;
                    this.ConcentrationString = "Stark";
                    break;
                default:
                    this.Concentration = Concentration.None;
                    this.ConcentrationString = "Keine";
                    break;
            }
        }
    }


    public enum Concentration { None, Low, Middle, High };
}
