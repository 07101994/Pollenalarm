using Pollenalarm.Old.WinPhone.Models;
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
using System.Windows.Threading;

namespace Pollenalarm.Old.WinPhone.Controls
{
    public partial class ConcentrationBar : UserControl
    {
        public ConcentrationBar()
        {
            InitializeComponent();
            concentrationBar.Opacity = 0;
        }

        public ConcentrationBar(int concentrationLvl)
        {
            InitializeComponent();
            concentrationBar.Opacity = 0;

            switch (concentrationLvl)
            {
                case 1:
                    concentrationBar.Width = 60;
                    break;

                case 2:
                    concentrationBar.Width = 130;
                    break;

                case 3:
                    concentrationBar.Width = 196;
                    break;
            }
        }

        public ConcentrationBar(Concentration c)
        {
            InitializeComponent();
            concentrationBar.Opacity = 0;

            switch (c)
            {
                case Concentration.Low:
                    concentrationBar.Width = 40;
                    break;

                case Concentration.Middle:
                    concentrationBar.Width = 80;
                    break;

                case Concentration.High:
                    concentrationBar.Width = 112;
                    break;
            }
        }

        public void Reset()
        {
            concentrationBar.Opacity = 0;
        }


        public void Animate()
        {
            AnimationStart.Begin();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Animate();
        }
    }
}
