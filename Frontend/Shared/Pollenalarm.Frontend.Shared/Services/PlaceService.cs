using Pollenalarm.Frontend.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class PlaceService
    {
        private IGeoLoactionService _GeoLocationService;
        private GoogleMapsService _GoogleMapsService;

        public PlaceService(IGeoLoactionService geoLocationService, GoogleMapsService googleMapsService)
        {
            _GeoLocationService = geoLocationService;
            _GoogleMapsService = googleMapsService;
        }

        public async Task<GeoLocation> GetCurrentGeoLocationAsync()
        {
            // Get GPS location
            var geoLocation = await _GeoLocationService.GetCurrentLocationAsync();
            if (geoLocation == null)
            {
                return null;
            }

            // Translate GPS to geocode
            var geocode = await _GoogleMapsService.ReverseGeocodeAsync(geoLocation);
            if (geocode == null)
            {
                return null;
            }

            return geocode;
        }

    }
}
