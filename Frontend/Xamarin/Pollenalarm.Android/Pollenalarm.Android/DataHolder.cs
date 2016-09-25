using System;
using System.Collections.Generic;
using Pollenalarm.Shared.ViewModels;
using System.IO;
using Android.Content;
using Newtonsoft.Json.Linq;


using Newtonsoft.Json;
using Android.Widget;
using System.Threading.Tasks;

namespace Pollenalarm.Droid
{
	public class DataHolder
	{
		public static DataHolder Current;

		public List<CityViewModel> CityList { get; set; }
		public List<PollutionViewModel> CurrentPollutions { get; set;}

		public CityViewModel CurrentCity { get; set; }
		public PollenViewModel CurrentPollen { get; set; }

		public DataHolder()
		{
			// Initialize lists
			CityList = new List<CityViewModel> ();
			CurrentPollutions = new List<PollutionViewModel> ();

			Current = this;
		}

		public void LoadCityList(Context context)
		{
			try
			{		
				using (var stream = new StreamReader (context.OpenFileInput("city.json")))
				{
					var json = stream.ReadToEnd();
					var parsed = JsonConvert.DeserializeObject<List<CityViewModel>>(json);
					CityList = parsed;
				}
			}
			catch (Java.IO.FileNotFoundException)
			{
				Console.WriteLine("Could not load saved cities out of JSON file.");
			}
		}

		public void SaveCityList(Context context)
		{
			var json = JsonConvert.SerializeObject(CityList);
			using (var stream = new StreamWriter(context.OpenFileOutput("city.json", FileCreationMode.Private)))
				stream.Write(json);
		}
	}
}

