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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Marketplace;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;

namespace Pollenalarm.Old.WinPhone.Views
{
    public partial class HelpAndInformation : PhoneApplicationPage
    {
        private LicenseInformation license;

        public HelpAndInformation()
        {
            InitializeComponent();
            license = new LicenseInformation();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnimationStart.Begin();

            if (App.IsPromo == true)
            {
                tbkPromo.Visibility = Visibility.Visible;
            }

            if (license.IsTrial() == false)
            {
                if (ApplicationBar.Buttons.Count > 2)
                    ApplicationBar.Buttons.Remove(ApplicationBar.Buttons[2]);
				
				pivot.Items.Remove(pivotBuy);
            }



            CheckSettings();
        }

        private void CheckSettings()
        {
            // Background Image
            if (App.Settings.Contains("BackgroundImage"))
            {
                if (((bool)App.Settings["BackgroundImage"]) == true)
                    BackgroundImage.Opacity = 1;
                else
                    BackgroundImage.Opacity = 0;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            AnimationEnd.Begin();
        }


        private void appbarContact_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Möchten Sie dem Entwickler ein E-Mail Feedback zukommen lassen?", "E-Mail Feedback", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.OK)
            {
                EmailComposeTask mail = new EmailComposeTask();
                mail.To = "support@thepagedot.de";
                mail.Subject = "Pollenalarm Feedback - Version " + textBlock2.Text;
                mail.Show();
            }
        }

        private void appBarRate_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask mrt = new MarketplaceReviewTask();
            mrt.Show();
        }

        private void appBarBuy_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.Show();
        }

        private void rbFacebook_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("http://www.facebook.com/Thepagedot");
            wbt.Show();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.Show();
        }

        private void appBarPromoCode_Click(object sender, EventArgs e)
        {
            if (App.IsPromo)
            {
                MessageBox.Show("Promotionscode wurde bereits aktiviert.", "Promotionscode", MessageBoxButton.OK);
            }
            else
            {
                InputPrompt input = new InputPrompt();
                input.Completed += new EventHandler<PopUpEventArgs<string, PopUpResult>>(input_Completed);
                input.Title = " Promotionscode";
                input.Message = "  Bitte geben Sie Ihren Promotionscode ein";
                input.Show();

                SystemTray.Opacity = 0.65;
            }
        }

        void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            SystemTray.Opacity = 0;

            if (e.Result != null)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri("http://thepagedot.de/pollenalarm/pollen.php?do=checkPromoCode&code=" + e.Result.ToString().ToUpper().Trim()));
            }
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            XDocument promoResult = XDocument.Parse(e.Result);

            foreach (XElement xmlPromoAnswer in promoResult.Descendants())
            {
                if (xmlPromoAnswer.Name == "error")
                {
                    MessageBox.Show(xmlPromoAnswer.Value, "Promotionscode", MessageBoxButton.OK);
                }
                else if (xmlPromoAnswer.Name == "result" && xmlPromoAnswer.Value.Equals("ok"))
                {
                    App.IsPromo = true;
                    App.SaveToSettings("promo", true);
                    tbkPromo.Visibility = Visibility.Visible;
                    MessageBox.Show("Promotionscode erfolgreich aktiviert.", "Promotionscode", MessageBoxButton.OK);
                }
            }            
        }
    }
}