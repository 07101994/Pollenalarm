using System;
using System.Threading.Tasks;
using Pollenalarm.Core;
using Pollenalarm.Frontend.Shared.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared
{
	public class GoogleMapsService
	{
		private IHttpService _HttpService;

		public GoogleMapsService(IHttpService httpService)
		{
			_HttpService = httpService;
		}

        public async Task<GeoLocation> ReverseGeocodeAsync(GeoLocation geoLocation)
		{
			try
			{
				var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={geoLocation.Latitute},{geoLocation.Longitute}&key={AccessKeys.GoogleMapsApiKey}";
				//var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key={AccessKeys.GoogleMapsApiKey}";
				var json = await _HttpService.GetStringAsync(url);

                var reverseGeocode = JsonConvert.DeserializeObject<GoogleGeoLocation>(json);
				if (reverseGeocode == null || reverseGeocode.Results?.Count == 0)
					return null;

				geoLocation.Name = reverseGeocode.Results[0].address_components.First(c => c.types.Contains("locality") && c.types.Contains("political")).long_name;
				geoLocation.Zip = reverseGeocode.Results[0].address_components.First(c => c.types.Contains("postal_code")).long_name;
				return geoLocation;
			}
			catch (Exception e)
			{
				return null;
			}
		}

        public async Task<IEnumerable<Place>> GeoCodeAsync(string searchTerm)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={searchTerm}&key={AccessKeys.GoogleMapsApiKey}";
                var json = await _HttpService.GetStringAsync(url);

                var googleGeoLocation = JsonConvert.DeserializeObject<GoogleGeoLocation>(json);
                if (googleGeoLocation == null || googleGeoLocation.Results?.Count == 0)
                    return null;

                var places = new List<Place>();
                foreach (var location in googleGeoLocation.Results)
                {
                    var name = location.address_components[0].long_name;
                    var zip = TryGetZipCode(location);

                    // Check if ZIP code could have been identified
                    if (zip == null)
                    {
                        // Place did not get ZIP code, because the search has not been precise enough
                        // Use place's coordinates to get one
                        var zipUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={location.geometry.location.lat},{location.geometry.location.lng}&key={AccessKeys.GoogleMapsApiKey}";
                        var zipJson = await _HttpService.GetStringAsync(zipUrl);

                        var reverseGeocode = JsonConvert.DeserializeObject<GoogleGeoLocation>(zipJson);
                        if (reverseGeocode == null || reverseGeocode.Results?.Count == 0)
                            continue;

                        zip = TryGetZipCode(reverseGeocode.Results[0]);
                        if (zip == null)
                            continue;
                    }

                    places.Add(new Place { Id = new Guid(), Name = name, Zip = zip });
                }

                return places;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string TryGetZipCode(Result location)
        {
            var zipResult = location.address_components.FirstOrDefault(c => c.types.Contains("postal_code"));
            if (zipResult == null)
                return null;

            return zipResult.long_name;
        }
    }
}
