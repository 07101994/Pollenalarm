using System;
using System.Threading.Tasks;
using Android.Graphics;
using System.Net;

namespace Pollenalarm.Droid
{
	public static class Helper
	{
		public static string ClinicalPollutionToString(Android.Content.Context context, int pollution)
		{
			switch (pollution)
			{				
				case 1:
					return context.GetString(Resource.String.clinical_pollution_low);
				case 2:
					return context.GetString(Resource.String.clinical_pollution_intermediate);
				case 3:
					return context.GetString(Resource.String.clinical_pollution_high);
				case 4:
					return context.GetString(Resource.String.clinical_pollution_veryhigh);
			}

			return "Unknown";
		}

		public static string BloomTimeToString (Android.Content.Context context, DateTime bloomStart, DateTime bloomEnd)
		{
			var start = GetStringForDay (context, bloomStart.Day);
			var end = GetStringForDay (context, bloomEnd.Day);

			var result = start + " " + bloomStart.ToString ("MMMMM") + " " + context.GetString (Resource.String.bloom_time_until) + " " + end + " " + bloomEnd.ToString ("MMMM");
			return result;
		}

		private static string GetStringForDay (Android.Content.Context context, int day)
		{
			if (day == 1)
				return context.GetString (Resource.String.bloom_time_begin);
			else if (day > 27)
				return context.GetString (Resource.String.bloom_time_end);
			else
				return context.GetString (Resource.String.bloom_time_middle);
		}

		public static async Task<Bitmap> GetImageBitmapFromUrlAsync(Uri uri)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = await webClient.DownloadDataTaskAsync(uri);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}

        public static int GetImageIdForPollen(int pollenId)
		{
			switch (pollenId)
			{
				default:
				case 2: return Resource.Drawable.Ambrosia;
				case 3: return Resource.Drawable.Ampfer;
				case 4: return Resource.Drawable.Beifuss;
				case 5: return Resource.Drawable.Birke;
				case 6: return Resource.Drawable.Buche;
				case 7: return Resource.Drawable.Eiche;
				case 8: return Resource.Drawable.Erle;
				case 9: return Resource.Drawable.Graeser;
				case 10: return Resource.Drawable.Hasel;
				case 11: return Resource.Drawable.Pappel;
				case 12: return Resource.Drawable.Roggen;
				case 13: return Resource.Drawable.Ulme;
				case 14: return Resource.Drawable.Wegerich;
				case 15: return Resource.Drawable.Weide;				
			}
		}

		public static int GetStringIdForPollution(int pollution)
		{
			switch (pollution)
			{
				default:
				case 0: return Resource.String.pollution_none;
				case 1: return Resource.String.pollution_low;
				case 2: return Resource.String.pollution_medium;
				case 3: return Resource.String.pollution_high;
			}
		}
	}
}

