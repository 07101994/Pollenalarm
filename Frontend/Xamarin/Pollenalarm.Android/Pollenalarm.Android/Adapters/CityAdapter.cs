using System;
using Android.Widget;
using Pollenalarm.Shared.ViewModels;
using Android.Content;
using System.Collections.Generic;
using Java.Lang;
using Android.Views;

namespace Pollenalarm.Droid
{
	public class CityAdapter : ArrayAdapter<CityViewModel>
	{
		private LayoutInflater layoutInflater;

		public CityAdapter(Context context, int resourceId, List<CityViewModel> items) : base(context, resourceId, items)
		{
			layoutInflater = LayoutInflater.From(context);				
		}

		public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View view = convertView;
			if (view == null)
				view = layoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = GetItem(position).Name;
			view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = GetItem(position).Zip;
			return view;
		}
	}
}