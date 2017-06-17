using System;
using Foundation;
using UIKit;
using Pollenalarm.Frontend.Forms.CustomControls;
using Pollenalarm.Frontend.Forms.iOS.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace Pollenalarm.Frontend.Forms.iOS.CustomRenderers
{
    public class HyperlinkLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // Set UILabel underlining
                var text = (NSMutableAttributedString)Control.AttributedText;
                text.AddAttribute(UIStringAttributeKey.UnderlineStyle, NSNumber.FromInt32((int)NSUnderlineStyle.Single), new NSRange(0, text.Length));
            }
        }
    }
}
