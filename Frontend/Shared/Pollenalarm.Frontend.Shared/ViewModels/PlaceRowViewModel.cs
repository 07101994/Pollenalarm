using System;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;
using System.Reflection;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class PlaceRowViewModel : AsyncViewModelBase
    {
        private IPollenService _PollenService;
        private Place _Place;

        public string Id { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; RaisePropertyChanged(); }
        }

        private string _Zip;
        public string Zip
        {
            get { return _Zip; }
            set { _Zip = value; RaisePropertyChanged(); }
        }

        private int _MaxPollutionToday;
        public int MaxPollutionToday
        {
            get { return _MaxPollutionToday; }
            set { _MaxPollutionToday = value; RaisePropertyChanged(); }
        }

        public bool _IsCurrentPosition;
        public bool IsCurrentPosition
        {
            get { return _IsCurrentPosition; }
            set { _IsCurrentPosition = value; RaisePropertyChanged(); }
        }

        public PlaceRowViewModel(IPollenService pollenService, Place place)
        {
            _PollenService = pollenService;
            UpdateProperties(place);
        }

        public void UpdateProperties(Place place)
        {
            _Place = place;
            Id = place.Id;
            Name = place.Name;
            Zip = place.Zip;
            IsCurrentPosition = place.IsCurrentPosition;
            MaxPollutionToday = place.MaxPollutionToday;
        }

        public async Task RefreshAsync()
        {
            await _PollenService.GetPollutionsForPlaceAsync(_Place);
            _Place.RecalculateMaxPollution();
            MaxPollutionToday = _Place.MaxPollutionToday;
        }
    }
}
