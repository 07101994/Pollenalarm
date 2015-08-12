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
using Android.Graphics.Drawables;
using Java.Util.Concurrent;
using System.Threading;

namespace Pollenalarm.Droid
{
	class PollutionViewHolder : Java.Lang.Object
	{
		public TextView Name { get; set; }
		public TextView Pollution { get; set; }
		public ImageView Background { get; set; }
		public int Position { get; set; }
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
				holder = new PollutionViewHolder();
				view = inflater.Inflate(Resource.Layout.PollutionItem, null);

				holder.Name = view.FindViewById<TextView>(Resource.Id.tvName);
				holder.Pollution = view.FindViewById<TextView>(Resource.Id.tvPollution);
				holder.Background = view.FindViewById<ImageView>(Resource.Id.ivBackground);
				view.Tag = holder;
			}

			// Fill view elements
			var valueString = "";
			switch (dayNumber) 
			{
				case 0:				
					valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueToday));
					break;
				case 1:
					valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueTomorrow));
					break;
				case 2:
					valueString = inflater.Context.GetString(Helper.GetStringIdForPollution(GetItem(position).ValueAfterTomorrow));
					break;				
			}

			holder.Name.Text = GetItem(position).Pollen.Name;
			holder.Pollution.Text = valueString;
			holder.Position = position;

            if (!fragment.IsScrolling)
                holder.Background.SetImageResource(Helper.GetImageIdForPollen(GetItem(position).Pollen));
            else
                holder.Background.SetImageResource(Android.Resource.Drawable.IcMenuGallery);
			    //new ImageLoaderTask(position, holder, inflater.Context).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, null);
					
			return view;
		}
	}

//	class ImageLoaderTask : AsyncTask
//	{
//		PollutionViewHolder holder;
//		Context context;
//		int position;
//
//
//		public ImageLoaderTask(int position, PollutionViewHolder holder, Context context)
//		{
//			this.holder = holder;
//			this.position = position;
//			this.context = context;
//		}
//
//		/// <summary>
//		/// Called on a background thread when the task is executed.
//		/// </summary>
//		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
//		{
//			//return context.GetDrawable(Helper.GetImageIdForPollen(holder.Pollen));
//			//return Helper.GetImageIdForPollen(holder.Pollen);
//		}
//
//		/// <summary>
//		/// Once the image is downloaded, associates it to the imageView
//		/// </summary>
//		protected override void OnPostExecute(Java.Lang.Object result)
//		{
////			if (holder.Position == position)
////				holder.Background.SetImageDrawable((Drawable)result);
//		}
//	}
}

