using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services.Base;
using Pollenalarm.Shared.ViewModels;

namespace Pollenalarm.Backend.Services
{
    public class CityService : ServiceBase
    {
        private Table<City> cityTable;

        public CityService()
        {
            cityTable = DataContext.GetTable<City>();
        }

        /// <summary>
        /// Gets a single city by its zip code
        /// </summary>
        /// <param name="zip">Zip code of the city</param>
        /// <returns>Single city</returns>
        public ServiceResult<CityViewModel> GetCity(string zip)
        {
            // Get city from database
            var city = cityTable.FirstOrDefault(c => Equals(c.Zip, zip));
            if (city == null)
                return new ServiceResult<CityViewModel>("No city with this zip code could be found.");

            // Convert city to view model
            var cityViewModel = city.ToCityViewModel();
            return new ServiceResult<CityViewModel>(cityViewModel);
        }

        /// <summary>
        /// Gets all 'favorite' cities that appears on the map
        /// </summary>
        /// <returns>List of cities</returns>
        public ServiceResult<List<CityViewModel>> GetMapCities()
        {
            // Get cities where the favorite flag is set
            var cities = cityTable.Where(c => c.IsFavorite == 1);
            if (!cities.Any())
                return new ServiceResult<List<CityViewModel>>("No cities found.");

            // Convert cities to view models
            var cityViewModels = cities.Select(c => c.ToCityViewModel());
            return new ServiceResult<List<CityViewModel>>(cityViewModels.ToList());
        }

        /// <summary>
        /// Adds a new city to the database or increases the user count if the city has alredy been existant.
        /// </summary>
        /// <param name="cityToAdd">New city to add to the database</param>
        /// <returns>The new or already existant city</returns>
        public ServiceResult<CityViewModel> AddCity(CityViewModel cityToAdd)
        {
            // Check if city has already been added
            var alreadyExistantCity = cityTable.FirstOrDefault(c => c.Zip == cityToAdd.Zip);
            if (alreadyExistantCity == null)
            {
                // Add city
                var city = City.FromViewModel(cityToAdd);
                cityTable.InsertOnSubmit(city);
                DataContext.SubmitChanges();
                return new ServiceResult<CityViewModel>(city.ToCityViewModel());
            }
            else
            {
                // Increment user count
                alreadyExistantCity.UserCount++;
                DataContext.SubmitChanges();
                return new ServiceResult<CityViewModel>(alreadyExistantCity.ToCityViewModel());
            }
        }
    }
}