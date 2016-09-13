using System;
using System.Threading.Tasks;
using Pollenalarm.Core;
using Pollenalarm.Frontend.Shared.Models;
using Newtonsoft.Json;
using System.Linq;

namespace Pollenalarm.Frontend.Shared
{
	public class GoogleMapsService
	{
		private IHttpService _HttpService;

		public GoogleMapsService(IHttpService httpService)
		{
			_HttpService = httpService;
		}

		public async Task<GeoLocation> ReverseGeocodeAsync(long longitude, long latitude)
		{
			try
			{
				//var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={longitude},{latitude}&key={AccessKeys.GoogleMapsApiKey}";
				var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key={AccessKeys.GoogleMapsApiKey}";
				var json = await _HttpService.GetStringAsync(url);
				var reverseGeocode = JsonConvert.DeserializeObject<GoogleGeoLocation>(json);

				if (reverseGeocode == null || reverseGeocode.Results?.Count == 0)
					return null;

				var geoLocation = new GeoLocation();
				geoLocation.Name = reverseGeocode.Results[0].address_components.First(c => c.types.Contains("administrative_area_level_1")).long_name;
				geoLocation.Zip = reverseGeocode.Results[0].address_components.First(c => c.types.Contains("postal_code")).long_name;;
				geoLocation.Latitute = latitude;
				geoLocation.Longitute = longitude;

				return geoLocation;
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
