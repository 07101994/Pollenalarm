using Pollenalarm.Frontend.Shared.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class PlaceService
    {
        private IFileSystemService _FileSystemService;
        private IGeoLoactionService _GeoLocationService;
        private GoogleMapsService _GoogleMapsService;

        public Place CurrentPlace { get; set; }
        public List<Place> Places { get; private set; }

        public PlaceService(IFileSystemService fileSystemService, IGeoLoactionService geoLocationService, GoogleMapsService googleMapsService)
        {
            _FileSystemService = fileSystemService;
            _GeoLocationService = geoLocationService;
            _GoogleMapsService = googleMapsService;

            Places = new List<Place>();
        }

        /// <summary>
        /// Loads the local places if needed.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if (!Places.Any())
            {
                var localPlaces = await _FileSystemService.ReadObjectFromFileAsync<List<Place>>("places.json");
                if (localPlaces != null && localPlaces.Any())
                    Places = localPlaces;
            }

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

        public async Task AddPlaceAsync(Place place)
        {
            // Add place to list of all places
            if (place.IsCurrentPosition)
                Places.Insert(0, place);
            else
                Places.Add(place);

            // Save changes locally
            await _FileSystemService.SaveObjectToFileAsync("places.json", Places.ToList());
        }

        public async Task UpdatePlaceAsync(Place place)
        {
            // Update
            var existingPlace = Places.FirstOrDefault(x => x.Id == place.Id);
            if (existingPlace != null)
            {
                existingPlace.Name = place.Name;
                existingPlace.Zip = place.Zip;
            }

            // Save changes
            await _FileSystemService.SaveObjectToFileAsync("places.json", Places.ToList());
        }

        public async Task DeletePlaceAsync(Place place)
        {
            var existingPlace = Places.FirstOrDefault(x => x.Id == place.Id);
            if (existingPlace != null)
            {
                Places.Remove(existingPlace);

                // Save changes
                await _FileSystemService.SaveObjectToFileAsync("places.json", Places.ToList());
            }
        }
    }
}
