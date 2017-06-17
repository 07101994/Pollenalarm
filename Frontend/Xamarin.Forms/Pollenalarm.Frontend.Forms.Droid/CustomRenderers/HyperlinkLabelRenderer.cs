using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text.Util;
using Pollenalarm.Frontend.Forms.Droid.CustomRenderers;
using Pollenalarm.Frontend.Forms.CustomControls;
using Android.Text;
using Android.Text.Style;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Util;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace Pollenalarm.Frontend.Forms.Droid.CustomRenderers
{
    public class HyperlinkLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // Set TextView underlining
                Control.PaintFlags |= Android.Graphics.PaintFlags.UnderlineText;
            }
        }
    }
}