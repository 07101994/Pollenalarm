using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pollenalarm.Shared.ViewModels;

namespace Pollenalarm.Backend.Models
{
    public static class ViewModelConverter
    {
        public static PollutionViewModel ToPollutionViewModel(this Pollution pollution, City city, Pollen pollen)
        {
            return new PollutionViewModel(
                pollution.Id,
                pollution.ValueToday,
                pollution.ValueTomorrow,
                pollution.ValueAfterTomorrow,
                pollution.TimeStamp,
                city.ToCityViewModel(),
                pollen.ToPollenViewModel()
            );
        }

        public static CityViewModel ToCityViewModel(this City city)
        {
            if (city == null)
                return null;

            return new CityViewModel(
                city.Zip,
                city.Name,
                city.LastUpdate,
                city.IsFavorite,
                city.UserCount
            );
        }

        public static PollenViewModel ToPollenViewModel(this Pollen pollen)
        {
            return new PollenViewModel(
                pollen.Id,
                pollen.Name,
                pollen.BloomStart,
                pollen.BloomEnd,
                pollen.ClinicalPollution,
                pollen.ImageUrl,
                pollen.ImageCredits,
                pollen.Description
            );
        }
    }
}