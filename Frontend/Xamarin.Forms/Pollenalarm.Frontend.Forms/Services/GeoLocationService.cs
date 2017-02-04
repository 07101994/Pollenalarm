using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Forms.Services
{
	public class GeoLocationService : IGeoLoactionService
	{
		private IGeolocator _Locator;

		public GeoLocationService()
		{

		}

		public async Task<GeoLocation> GetCurrentLocationAsync()
		{
			try
			{
				_Locator = CrossGeolocator.Current;
				_Locator.DesiredAccuracy = 50;
				var position = await _Locator.GetPositionAsync(timeoutMilliseconds: 10000);

				var geoLocation = new GeoLocation();
				geoLocation.Latitute = position.Latitude;
				geoLocation.Longitute = position.Longitude;

				return geoLocation;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
				return null;
			}
		}
	}
}
