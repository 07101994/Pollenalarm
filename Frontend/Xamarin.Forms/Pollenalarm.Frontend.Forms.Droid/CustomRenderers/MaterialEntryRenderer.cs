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
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Pollenalarm.Frontend.Forms.Droid.CustomRenderers
{
    public class MaterialEntryRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<Xamarin.Forms.Entry, Android.Views.View>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {

            }
        }
    }
}