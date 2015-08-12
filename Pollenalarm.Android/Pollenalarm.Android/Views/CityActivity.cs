
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media.Audiofx;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pollenalarm.Droid.Misc;
using Pollenalarm.Shared;
using System.Security.Principal;

namespace Pollenalarm.Droid
{
	[Activity(Label = "City", ParentActivity = typeof(MainActivity))]			
	public class CityActivity : Activity
	{
		PollutionService pollutionService;

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

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayHomeAsUpEnabled(true);

			SetContentView(Resource.Layout.City);
		}

		protected override async void OnResume()
		{
			base.OnResume ();

			Title = DataHolder.Current.CurrentCity.Name;

			// Load pollutions
			pollutionService = new PollutionService(Settings.ApiBaseUrl);
			var pollutions = await pollutionService.GetPollutionForCity(DataHolder.Current.CurrentCity.Zip);
			if (pollutions == null) 
			{
				// An error occured
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GetString(Resource.String.error_header));
				builder.SetMessage(GetString(Resource.String.pollution_error_body));
				builder.SetPositiveButton (GetString (Android.Resource.String.Ok), (senderAlert, args) => {});
				builder.Show();
			} else {
				DataHolder.Current.CurrentPollutions = pollutions;
			}				

			// Initialize tabs
			AddPollutionDayTab(Resources.GetString(Resource.String.tab_today), 0);
			AddPollutionDayTab(Resources.GetString(Resource.String.tab_tomorrow), 1);
			AddPollutionDayTab(Resources.GetString(Resource.String.tab_aftertomorrow), 2);

			// Hide loading indicator
			var loading = FindViewById<ProgressBar> (Resource.Id.pbLoading);
			loading.Visibility = ViewStates.Gone;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.city_activity_actions, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnMenuItemSelected (int featureId, IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_edit_city:
					var intent = new Intent (this, typeof(AddEditCityActivity));
					intent.PutExtra ("isEditMode", true);
					StartActivity (intent);
					return true;
			}

			return base.OnMenuItemSelected (featureId, item);
		}

		void AddPollutionDayTab(string tabText, int dayNumber)
		{
			// Create tab fragment
			var fragment = new PollutionListFragment ();

			// Add day number as argument to the fragment
			var bundle = new Bundle();
			bundle.PutInt ("dayNumber", dayNumber);
			fragment.Arguments = bundle;

			// Create new tab with fragment
			var tab = ActionBar.NewTab ();
			tab.SetText (tabText);
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Replace(Resource.Id.fragmentContainer, fragment);
			};

			ActionBar.AddTab (tab);
		}
	}
}

