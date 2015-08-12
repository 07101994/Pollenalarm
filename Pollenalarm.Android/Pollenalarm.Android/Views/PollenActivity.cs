
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
using Android.Graphics;
using System.Net;
using System.Threading.Tasks;

namespace Pollenalarm.Droid
{
	[Activity(Label = "PollenActivity", ParentActivity=typeof(CityActivity))]			
	public class PollenActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);		
			// Hide image on Lollypop
			if (Build.VERSION.SdkInt <= BuildVersionCodes.Kitkat)
			{
				ActionBar.SetLogo (Resource.Drawable.Icon);
				ActionBar.SetDisplayUseLogoEnabled (true);
				ActionBar.SetDisplayShowHomeEnabled(true);
			}

			ActionBar.SetDisplayHomeAsUpEnabled(true);

			SetContentView(Resource.Layout.Pollen);
			Title = DataHolder.Current.CurrentPollen.Name;

			// Image
			var image = FindViewById<ImageView>(Resource.Id.pollenDetailsImage);
			image.SetImageResource(Helper.GetImageIdForPollen(DataHolder.Current.CurrentPollen));
			image.SetScaleType (ImageView.ScaleType.FitXy);
			image.SetAdjustViewBounds (true);

			// Description
			var description = FindViewById<TextView>(Resource.Id.pollenDetailsDescription);
			description.Text = DataHolder.Current.CurrentPollen.Description;

			// Pollution
			var clinicalPollution = FindViewById<TextView>(Resource.Id.pollenClinicalPollution);
			clinicalPollution.Text = Helper.ClinicalPollutionToString(this, DataHolder.Current.CurrentPollen.ClinicalPollution);

			// Bloom Time
			var bloomTime = FindViewById<TextView> (Resource.Id.pollenBloomTime);
			bloomTime.Text = Helper.BloomTimeToString (this, DataHolder.Current.CurrentPollen.BloomStart, DataHolder.Current.CurrentPollen.BloomEnd);
		}			
	}
}