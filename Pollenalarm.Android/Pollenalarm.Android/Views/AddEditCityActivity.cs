
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
	public class AddEditCityActivity : Activity
	{
		private bool isEditMode = false;
		private EditText etCityName;
		private EditText etCityZip;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			// Hide image on Lollypop
			if (Build.VERSION.SdkInt <= BuildVersionCodes.Kitkat)
			{
				ActionBar.SetLogo (Resource.Drawable.Icon);
				ActionBar.SetDisplayUseLogoEnabled (true);
				ActionBar.SetDisplayShowHomeEnabled(true);
			}
				
			ActionBar.SetDisplayHomeAsUpEnabled(true);

			SetContentView(Resource.Layout.AddEditCity);

			// Get view elements
			etCityName = FindViewById<EditText> (Resource.Id.addEditCityName);
			etCityZip = FindViewById<EditText> (Resource.Id.addEditCityZip);

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
				case Resource.Id.action_add_city_confirm:
					if (isEditMode)
						EditCity();
					else
						AddCity();
					return true;
				case Resource.Id.action_delete_city:
					DeleteCity();
					return true;				
			}

			return base.OnMenuItemSelected(featureId, item);
		}

		bool CheckInputs ()
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
			var city = new CityViewModel (
				((EditText)FindViewById (Resource.Id.addEditCityZip)).Text,
				((EditText)FindViewById (Resource.Id.addEditCityName)).Text,
				null,
				0,
				0
			);

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
	}
}