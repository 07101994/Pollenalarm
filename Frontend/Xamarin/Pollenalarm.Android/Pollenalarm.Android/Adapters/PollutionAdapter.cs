using System;
using Android.Widget;
using Pollenalarm.Shared.ViewModels;
using Android.Views;
using Android.App;
using Android.Content.Res;
using Android.Content;
using System.Collections.Generic;
using Java.Lang;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Util.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Square.Picasso;

namespace Pollenalarm.Droid
{
	class PollutionViewHolder : Java.Lang.Object
	{
		public TextView Name { get; set; }
		public TextView Pollution { get; set; }
		public ImageView Background { get; set; }
	}

	public class PollutionAdapter : ArrayAdapter<PollutionViewModel>
	{
        private PollutionListFragment fragment;
		private LayoutInflater inflater;
		private int dayNumber;

        public PollutionAdapter(PollutionListFragment fragment, Context context, int resourceId, List<PollutionViewModel> items, int dayNumber) : base(context, resourceId, items)
		{
            this.fragment = fragment;
			this.inflater = LayoutInflater.From(context);			
			this.dayNumber = dayNumber;
		}

		public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			PollutionViewHolder holder = null;
			var view = convertView;

			// Prepare view holder
			if (view != null)
				holder = view.Tag as PollutionViewHolder;

			if (holder == null)
			{
                view = inflater.Inflate(Resource.Layout.PollutionItem, null);

				holder = new PollutionViewHolder();				
				holder.Name = view.FindViewById<TextView>(Resource.Id.tvName);
				holder.Pollution = view.FindViewById<TextView>(Resource.Id.tvPollution);
				holder.Background = view.FindViewById<ImageView>(Resource.Id.ivBackground);

				view.Tag = holder;
			}
                			
            // Decide which value to display according to the dayNumber
			var valueString = "Test";
			switch (dayNumber) 
			{
				case 0:	valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueToday)); break;
				case 1: valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueTomorrow)); break;
				case 2: valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueAfterTomorrow)); break;				
			}

            // Fill holder elements
            var pollution = GetItem(position);
            holder.Name.Text = pollution.Pollen.Name;
			holder.Pollution.Text = valueString;
            var width = fragment.View.Width;
            var height = 120 * Context.Resources.DisplayMetrics.Density;
            Picasso.With(Context).Load(Helper.GetImageIdForPollen(GetItem(position).Pollen.Id)).Resize((int)width, (int)height).CenterCrop().Into(holder.Background);
					
			return view;
		}            
    }
}