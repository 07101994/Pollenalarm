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
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Marketplace;
using Microsoft.Advertising.Mobile.UI;

namespace Pollenalarm.Old.WinPhone.Controls
{
    public partial class RemovableAdvert : UserControl
    {
        private AdControl adControl;
        private bool isAdvertLoaded, isDeaktivatedOnFullVersion;
        private LicenseInformation license;
        public string ApplicationId, AdUnitId;

        /// <summary>
        /// Advertising Panel with Button to Hide
        /// </summary>
        /// <param name="applicationId">Application ID</param>
        /// <param name="adUnitId">Unit ID</param>
        /// /// <param name="isDeaktivatedOnFullVersion">Does the full Version of your app deaktivates the adverts?</param>
        public RemovableAdvert(string applicationId, string adUnitId, bool isDeaktivatedOnFullVersion)
        {
            InitializeComponent();

            this.isAdvertLoaded = false;
            this.ApplicationId = applicationId;
            this.AdUnitId = adUnitId;
            this.isDeaktivatedOnFullVersion = isDeaktivatedOnFullVersion;
            this.license = new LicenseInformation();

            // Check whether trial may be displayed.
            if (license.IsTrial() == false && isDeaktivatedOnFullVersion)
            {
                adControl = null;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                adControl = GetNewAdControl();
            }            
        }

        /// <summary>
        /// Returns a new AdControl
        /// </summary>
        /// <returns></returns>
        private AdControl GetNewAdControl()
        {
            AdControl adControl = new AdControl(ApplicationId, AdUnitId, true);
            adControl.Width = 480;
            adControl.Height = 80;
            adControl.AdRefreshed += new EventHandler(adControl_AdRefreshed);
            adControl.ErrorOccurred += new EventHandler<Microsoft.Advertising.AdErrorEventArgs>(adControl_ErrorOccurred);

            return adControl;
        }

        void adControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            AnimationShowRemoveButton.Begin();
        }

        void adControl_AdRefreshed(object sender, EventArgs e)
        {
            if (isAdvertLoaded == false)
            {
                AnimationShowRemoveButton.Begin();
                isAdvertLoaded = true;
            }
        }

        /// <summary>
        /// Adds the AdControl to the Advertising Panel
        /// Imvoke this in OnNavigatedTo
        /// </summary>
        public void AddAdControl()
        {
            // Check license
            if (license.IsTrial() == false && isDeaktivatedOnFullVersion)
            {
                adControl = null;
                this.Visibility = Visibility.Collapsed;
            }

            // Add adControl
            if (adControl != null)
            {
                if (!AdPanel.Children.Contains(adControl))
                {
                    AdPanel.Children.Add(adControl);
                }
            }
        }

        /// <summary>
        /// Removes AdControl from Advertising Panel. Important for avoiding error after navigating from page.
        /// Invoke this method in OnNavigatedFrom
        /// </summary>
        public void RemoveAdControl()
        {
            if (adControl != null)
            {
                AdPanel.Children.Remove(adControl);
            }
        }

        private void btnHideAdvert_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

            int hideAdvertCount;

            if (App.Settings.Contains("HideAdvertCount"))
            {
                hideAdvertCount = (int)App.LoadFromSettings("HideAdvertCount", null);
            }
            else
            {
                hideAdvertCount = 1;
            }

            if (hideAdvertCount >= 5 && license.IsTrial())
            {
                MessageBoxResult result = MessageBox.Show("Die eingeblendeten Werbeanzeigen scheinen Sie zu stören. Klicken Sie auf \"Ok\" um die Vollversion zu kaufen und die Werbeanzeigen dauerhaft auszublenden.", "Werbung dauerhaft ausblenden", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
                    marketplaceDetailTask.Show();
                }

                hideAdvertCount = 0;
            }

            hideAdvertCount++;
            App.SaveToSettings("HideAdvertCount", hideAdvertCount);
        }
    }
}
