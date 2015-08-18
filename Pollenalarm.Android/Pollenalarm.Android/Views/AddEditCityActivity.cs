
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
using Java.Security;
using System.Runtime.InteropServices;
using Pollenalarm.Shared.ViewModels;
using Java.Util;
using Android.Locations;

namespace Pollenalarm.Droid
{
	[Activity (Label = "Add a new city", ParentActivity = typeof(MainActivity))]			
    public class AddEditCityActivity : Activity, ILocationListener
	{
		private bool isEditMode = false;
		private EditText etCityName;
		private EditText etCityZip;

        // Location
        private LocationManager locationManager;
        private Location location;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
            SetContentView(Resource.Layout.AddEditCity);

			// Hide image on Lollypop
			if (Build.VERSION.SdkInt <= BuildVersionCodes.Kitkat)
			{
				ActionBar.SetLogo (Resource.Drawable.Icon);
				ActionBar.SetDisplayUseLogoEnabled (true);
				ActionBar.SetDisplayShowHomeEnabled(true);
			}				
			ActionBar.SetDisplayHomeAsUpEnabled(true);
            			
			// Get view elements
			etCityName = FindViewById<EditText> (Resource.Id.addEditCityName);
			etCityZip = FindViewById<EditText> (Resource.Id.addEditCityZip);
            var btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += (sender, e) => {
                if (isEditMode)
                    EditCity();
                else
                    AddCity();
            };

            locationManager = (LocationManager)GetSystemService(LocationService);

			// Check if edit mode
			isEditMode = Intent.GetBooleanExtra("isEditMode", false);
			if (isEditMode)
			{				
				// Set title
				Title = "Edit " + DataHolder.Current.CurrentCity.Name;

				// Load data from current city
				var name = FindViewById<EditText>(Resource.Id.addEditCityName);
				name.Text = DataHolder.Current.CurrentCity.Name;
				var zip = FindViewById<EditText>(Resource.Id.addEditCityZip);
				zip.Text = DataHolder.Current.CurrentCity.Zip;
			}                
		}

        protected override void OnResume()
        {
            base.OnResume();

            // Subscribe location changes
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 0, this);
            locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 1000, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Unsibscribe location changes
            locationManager.RemoveUpdates(this);
        }

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.addeditcity_activity_actions, menu);

			// Hide delete button if in edit mode
			var delete = menu.FindItem(Resource.Id.action_delete_city);
			delete.SetVisible(isEditMode);

			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnMenuItemSelected(int featureId, IMenuItem item)
		{
			switch (item.ItemId)
			{
                case Resource.Id.action_location:
                    GetCurrentPosition();
                    break;
				case Resource.Id.action_delete_city:
					DeleteCity();
					return true;				
			}

			return base.OnMenuItemSelected(featureId, item);
		}

        async void GetCurrentPosition()
        {
            // Get current address
            var geoCoder = new Geocoder(this);
            var addressList = await geoCoder.GetFromLocationAsync(location.Latitude, location.Longitude, 1);
            var address = addressList.FirstOrDefault();
            if (address != null)
            {
                etCityName.Text = address.Locality;
                etCityZip.Text = address.PostalCode;
            }
            else
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle (Resource.String.location_not_found_title);
                builder.SetMessage(Resource.String.location_not_found_body);
                builder.SetNeutralButton(Android.Resource.String.Ok, (s, e) => {});
                builder.Show();
            }
        }

		bool CheckInputs()
		{
			if (etCityZip.Text.Length != 5)
				return false;
			if (etCityName.Text.Length == 0)
				return false;
			return true;
		}

		/// <summary>
		/// Transforms the current inputs into a city model and adds it to the list.
		/// </summary>
		void AddCity()
		{
			// Check inputs
			if (!CheckInputs())
			{
				ShowInputError();
				return;
			}

			// Create city from inputs
			var city = new CityViewModel (((EditText)FindViewById (Resource.Id.addEditCityZip)).Text, ((EditText)FindViewById (Resource.Id.addEditCityName)).Text, null, 0, 0);

			// Add city to list
			DataHolder.Current.CurrentCity = city;
			DataHolder.Current.CityList.Add(city);
			DataHolder.Current.SaveCityList(this);

			// Finish activity
			Finish();
		}

		void EditCity()
		{
			// Check inputs
			if (!CheckInputs())
			{
				ShowInputError();
				return;
			}

			var index = DataHolder.Current.CityList.IndexOf(DataHolder.Current.CurrentCity);
			if (index >= 0) 
			{
				DataHolder.Current.CityList[index].Name = FindViewById<EditText> (Resource.Id.addEditCityName).Text;
				DataHolder.Current.CityList[index].Zip = FindViewById<EditText> (Resource.Id.addEditCityZip).Text;
				DataHolder.Current.SaveCityList(this);
			}

			// Finish activity
			Finish();
		}

		void DeleteCity ()
		{
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle (String.Format(GetString(Resource.String.delete_city_title), DataHolder.Current.CurrentCity.Name));
			builder.SetMessage (GetString(Resource.String.delete_city_body));
			builder.SetPositiveButton("Delete", (senderAlert, args) => 
			{
				DataHolder.Current.CityList.Remove(DataHolder.Current.CurrentCity);
				DataHolder.Current.SaveCityList(this);
				NavigateUpTo(ParentActivityIntent);
			});
			builder.SetNegativeButton ("Cancel", (senderAlert, args) => {});
			builder.Show();
		}

		void ShowInputError()
		{
			var message = new AlertDialog.Builder(this);
			message.SetTitle(GetText(Resource.String.addEdit_error_header));
			message.SetMessage(GetText (Resource.String.addEdit_error_body));
			message.SetPositiveButton(GetText (Android.Resource.String.Ok), (senderAlert, args) => {});
			message.Show();
		}
           
        // React on location changes
        public void OnProviderDisabled(string provider) {}
        public void OnProviderEnabled(string provider) {}
        public void OnStatusChanged(string provider, Availability status, Bundle extras) {}
        public void OnLocationChanged(Location location) 
        {
            this.location = location;
            locationManager.RemoveUpdates(this);
        }
	}
}