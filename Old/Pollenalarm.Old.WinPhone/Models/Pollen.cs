using Pollenalarm.Old.WinPhone.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Pollenalarm.Old.WinPhone.Models
{
    public class Pollen : NotifyClass
    {
        // ----------------------------------------
        // Properties
        // ----------------------------------------

        // Name of Pollentype
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { if (value != _Name) { _Name = value; NotifyPropertyChanged("Name"); } }
        }

        // Id Number
        private int _Id;
        public int Id
        {
            get { return _Id; }
            set { if (value != _Id) { _Id = value; NotifyPropertyChanged("Id"); } }
        }        

        // Concentration Level
        private Concentration _Concentration;
        public Concentration Concentration
        {
            get { return _Concentration; }
            set { if (value != _Concentration) { _Concentration = value; NotifyPropertyChanged("Concentration"); } }
        }

        private string _ConcentrationString;
        public string ConcentrationString
        {
            get { return _ConcentrationString; }
            set { if (value != _ConcentrationString) { _ConcentrationString = value; NotifyPropertyChanged("ConcentrationString"); } }
        }

        // Clinical Pollution Level
        private ClinicalPollution _ClinicalPollution;
        public ClinicalPollution ClinicalPollution
        {
            get { return _ClinicalPollution; }
            set { if (value != _ClinicalPollution) { _ClinicalPollution = value; NotifyPropertyChanged("ClinicalPollution"); } }
        }

        private string _ClinicalPollutionString;
        public string ClinicalPollutionString
        {
            get { return ClinicalPollutionToString(); }
            set { if (value != _ClinicalPollutionString) { _ClinicalPollutionString = value; NotifyPropertyChanged("ClinicalPollutionString"); } }
        }

        private string ClinicalPollutionToString()
        {
            switch (ClinicalPollution)
            {
                case ClinicalPollution.Low:
                    return "Gering";
                case ClinicalPollution.Middle:
                    return "Mäßig";
                case ClinicalPollution.High:
                    return "Stark";
                case ClinicalPollution.VeryHigh:
                    return "Sehr Stark";
                default:
                    return "Keine";
            }
        }

        // Flag whether this Pollentype is selected
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { if (value != _IsSelected) { _IsSelected = value; NotifyPropertyChanged("IsSelected"); } }
        }

        // Visibility depends on IsSelected      
        public Visibility Visibility
        {
            get 
            {
                if (IsSelected)
                {
                    if (App.ShowAllPollen)
                    {
                        return Visibility.Visible;
                    }
                    else if (Concentration > 0)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }            
        }

        // Color depends on Concentrarion      
        public SolidColorBrush Color
        {
            get
            {
                if (Concentration > 0)
                {
                    return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                }
                else
                {
                    return (SolidColorBrush)Application.Current.Resources["PhoneBorderBrush"];
                }
            }
        }


        // Information for Pollen Details

        private string _GeneralPollenInformation;
        public string GeneralPollenInformation
        {
            get { return _GeneralPollenInformation; }
            set { if (value != _GeneralPollenInformation) { _GeneralPollenInformation = value; NotifyPropertyChanged("GeneralPollenInformation"); } }
        }


        private string _BloomTime;
        public string BloomTime
        {
            get { return _BloomTime; }
            set { if (value != _BloomTime) { _BloomTime = value; NotifyPropertyChanged("BloomTime"); } }
        }


        private string _ImageCredits;
        public string ImageCredits
        {
            get { return _ImageCredits; }
            set { if (value != _ImageCredits) { _ImageCredits = value; NotifyPropertyChanged("ImageCredits"); } }
        }

        public string ImagePath
        {
            get { return "/Assets/Pollen/" + App.AsciName(Name) + ".png"; }
            //set { if (value != _ImagePath) { _ImagePath = value; NotifyPropertyChanged("_magePath"); } }
        }
        

        // ----------------------------------------
        // Constructors
        // ----------------------------------------

        public Pollen() { }

        public Pollen(string name, Concentration concentration, bool isSelected)
        {

            this.Name = name;
            this.Concentration = concentration;
            this.IsSelected = isSelected;
        }

        public Pollen(string name, string concentrationString, bool isSelected)
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

        public int ConcentrationToInt32()
        {
            switch (Concentration)
            {
                case Concentration.None:
                    return 0;
                case Concentration.Low:
                    return 1;
                case Concentration.Middle:
                    return 2;
                case Concentration.High:
                    return 3;
            }

            return 0;
        }
    }

    /// <summary>
    /// Concentration level of Pollentypes in specific areas
    /// </summary>
    public enum Concentration { None, Low, Middle, High };

    /// <summary>
    /// Defines how strong the clinical pollutin is and how strong allergicans react to this pollen
    /// </summary>
    public enum ClinicalPollution { Low, Middle, High, VeryHigh };
}
