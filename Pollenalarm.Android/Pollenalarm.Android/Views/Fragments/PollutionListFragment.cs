
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Pollenalarm.Droid
{
	public class PollutionListFragment : Fragment
	{
        public bool IsScrolling { get; set; }
        private ListView listView;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            var dayNumber = Arguments.GetInt ("dayNumber");
			var view = inflater.Inflate (Resource.Layout.PollutionList, container, false);

			listView = view.FindViewById<ListView>(Resource.Id.pollutionListView);			
			listView.Adapter = new PollutionAdapter(this, inflater.Context, -1, DataHolder.Current.CurrentPollutions, dayNumber);
			listView.ItemClick += ListView_ItemClick;
            listView.ScrollStateChanged += ListView_ScrollStateChanged;

			return view;
		}

        void ListView_ScrollStateChanged(object sender, AbsListView.ScrollStateChangedEventArgs e)
        {
            IsScrolling = (e.ScrollState != ScrollState.Idle);
            if (!IsScrolling)
                ((PollutionAdapter)listView.Adapter).NotifyDataSetChanged();
        }            

		void ListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			var selectedPollution = DataHolder.Current.CurrentPollutions.ElementAt(e.Position);
			if (selectedPollution != null)
			{
				DataHolder.Current.CurrentPollen = selectedPollution.Pollen;
				var intent = new Intent (Activity, typeof(PollenActivity));
				Activity.StartActivity(intent);
			}
		}
	}
}  