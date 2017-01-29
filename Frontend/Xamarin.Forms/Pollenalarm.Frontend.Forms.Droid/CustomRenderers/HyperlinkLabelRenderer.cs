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
using Pollenalarm.Frontend.Forms.CustomRenderers;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace Pollenalarm.Frontend.Forms.Droid.CustomRenderers
{
    public class HyperlinkLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            Linkify.AddLinks(Control, MatchOptions.All);
        }
    }
}