using Foundation;
using Pollenalarm.Frontend.Forms.CustomRenderers;
using Pollenalarm.Frontend.Forms.iOS.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace Pollenalarm.Frontend.Forms.iOS.CustomRenderers
{
    class HyperlinkLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            Control.TextColor = UIColor.Blue;

            Control.UserInteractionEnabled = true;

            var gesture = new UITapGestureRecognizer();

            gesture.AddTarget(() =>
            {
                var url = new NSUrl("https://" + Control.Text);

                if (UIApplication.SharedApplication.CanOpenUrl(url))
                    UIApplication.SharedApplication.OpenUrl(url);
            });

            Control.AddGestureRecognizer(gesture);
        }
    }
}
