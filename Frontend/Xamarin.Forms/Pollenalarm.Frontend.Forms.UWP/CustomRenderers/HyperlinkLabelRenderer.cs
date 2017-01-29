using Pollenalarm.Frontend.Forms.CustomRenderers;
using Pollenalarm.Frontend.Forms.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace Pollenalarm.Frontend.Forms.UWP.CustomRenderers
{
    class HyperlinkLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            Control.Foreground = new SolidColorBrush(Colors.Blue);
            Control.Tapped += LabelTapped;
        }

        private async void LabelTapped(object sender, TappedRoutedEventArgs e)
        {
            var website = Element.Text;

            if (website.IndexOf("http://") == -1)
                website = "http://" + website;

            await Launcher.LaunchUriAsync(new Uri(website));
        }
    }
}
