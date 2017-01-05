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
using Pollenalarm.Old.WinPhone.Helper;

namespace Pollenalarm.Old.WinPhone.Models
{
    public class MapConcentration : NotifyClass
    {
        //private string defaultColor = ((SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"]).Color.ToString();
        private string defaultColor = Colors.Gray.ToString();


        public ObservableCollection<string> ocMapColors = new ObservableCollection<string>();

        private string _Berlin;
        public string Berlin
        {
            get { return _Berlin; }
            set { if (value != _Berlin) { _Berlin = value; NotifyPropertyChanged("Berlin"); } }
        }

        private string _Bonn;
        public string Bonn
        {
            get { return _Bonn; }
            set { if (value != _Bonn) { _Bonn = value; NotifyPropertyChanged("Bonn"); } }
        }

        private string _Dresden;
        public string Dresden
        {
            get { return _Dresden; }
            set { if (value != _Dresden) { _Dresden = value; NotifyPropertyChanged("Dresden"); } }
        }

        private string _Frankfurt;
        public string Frankfurt
        {
            get { return _Frankfurt; }
            set { if (value != _Frankfurt) { _Frankfurt = value; NotifyPropertyChanged("Frankfurt"); } }
        }

        private string _Hamburg;
        public string Hamburg
        {
            get { return _Hamburg; }
            set { if (value != _Hamburg) { _Hamburg = value; NotifyPropertyChanged("Hamburg"); } }
        }

        private string _Hannover;
        public string Hannover
        {
            get { return _Hannover; }
            set { if (value != _Hannover) { _Hannover = value; NotifyPropertyChanged("Hannover"); } }
        }

        private string _Muenchen;
        public string Muenchen
        {
            get { return _Muenchen; }
            set { if (value != _Muenchen) { _Muenchen = value; NotifyPropertyChanged("Muenchen"); } }
        }

        private string _Nuernberg;
        public string Nuernberg
        {
            get { return _Nuernberg; }
            set { if (value != _Nuernberg) { _Nuernberg = value; NotifyPropertyChanged("Nuernberg"); } }
        }

        private string _Saarbruecken;
        public string Saarbruecken
        {
            get { return _Saarbruecken; }
            set { if (value != _Saarbruecken) { _Saarbruecken = value; NotifyPropertyChanged("Saarbruecken"); } }
        }

        private string _Stuttgart;
        public string Stuttgart
        {
            get { return _Stuttgart; }
            set { if (value != _Stuttgart) { _Stuttgart = value; NotifyPropertyChanged("Stuttgart"); } }
        }

        private string _Rostock;
        public string Rostock
        {
            get { return _Rostock; }
            set { if (value != _Rostock) { _Rostock = value; NotifyPropertyChanged("Rostock"); } }
        }

        public MapConcentration()
        {
            Berlin = defaultColor;
            Bonn = defaultColor;
            Dresden = defaultColor;
            Frankfurt = defaultColor;
            Hamburg = defaultColor;
            Hannover = defaultColor;
            Muenchen = defaultColor;
            Nuernberg = defaultColor;
            Saarbruecken = defaultColor;
            Stuttgart = defaultColor;
            Rostock = defaultColor;
        }

        internal void setValue(int counter, string value)
        {
            string valueToColor;
            switch (value)
            {
                default:
                    valueToColor = defaultColor;
                    break;
                case "-":
                    valueToColor = "#FFCCCCCC";
                    break;
                case "1":
                    valueToColor = "#FF009F2E";
                    break;
                case "2":
                    valueToColor = "#FFFFF500";
                    break;
                case "3":
                    valueToColor = "#FFFF0000";
                    break;
            }

            switch (counter)
            {
                case 0:
                    Berlin = valueToColor;
                    break;
                case 1:
                    Bonn = valueToColor;
                    break;
                case 2:
                    Dresden = valueToColor;
                    break;
                case 3:
                    Frankfurt = valueToColor;
                    break;
                case 4:
                    Hamburg = valueToColor;
                    break;
                case 5:
                    Hannover = valueToColor;
                    break;
                case 6:
                    Muenchen = valueToColor;
                    break;
                case 7:
                    Nuernberg = valueToColor;
                    break;
                case 8:
                    Saarbruecken = valueToColor;
                    break;
                case 9:
                    Stuttgart = valueToColor;
                    break;
                case 10:
                    Rostock = valueToColor;
                    break;
            }
        }
    }
}
