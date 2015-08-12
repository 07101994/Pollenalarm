
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Linq;

namespace Pollenalarm.Droid
{
	[Activity (Label = "Pollenalarm", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private CityAdapter testAdapter;

		protected override void OnCreate(Bundle bundle)
		{
			// Initialize DataHolder if needed
			if (DataHolder.Current == null)
				DataHolder.Current = new DataHolder();

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			DataHolder.Current.LoadCityList(this);

			var listView = (ListView)FindViewById(Resource.Id.cityListView);
			listView.ItemClick += ListView_ItemClick;

			testAdapter = new CityAdapter (this, 0, DataHolder.Current.CityList);

			var cityListView = (ListView)FindViewById(Resource.Id.cityListView);
			cityListView.Adapter = testAdapter;
		}	

		protected override void OnResume()
		{
			base.OnResume();

			// Update the list view
			testAdapter.Clear();
			testAdapter.AddAll(DataHolder.Current.CityList);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_acticity_actions, menu);
			return base.OnCreateOptionsMenu (menu);		
		}

		public override bool OnMenuItemSelected(int featureId, IMenuItem item)
		{
			switch (item.ItemId) 
			{
				case Resource.Id.action_add_city:
					var intent = new Intent (this, typeof(AddEditCityActivity));
					StartActivity (intent);
					return true;
			}

			return base.OnMenuItemSelected (featureId, item);
		}

		void ListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			DataHolder.Current.CurrentCity = DataHolder.Current.CityList.ElementAt(e.Position);
			var intent = new Intent (this, typeof(CityActivity));
			StartActivity(intent);
		}
	}
}