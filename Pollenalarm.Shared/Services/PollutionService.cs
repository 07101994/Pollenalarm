using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Pollenalarm.Shared.ViewModels;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Pollenalarm.Shared
{
	public class PollutionService
	{
		private readonly string baseUrl;

		public PollutionService(string baseUrl)
		{
			this.baseUrl = baseUrl;
		}

        /// <summary>
        /// Loads the latest pollution values for a single city from the database
        /// </summary>
        /// <param name="zip">City Zip Code</param>
        /// <returns>List of latest pollutions</returns>
		public async Task<List<PollutionViewModel>> GetPollutionForCity(string zip)
		{
			var result = await Downloader.DownloadAsync<List<PollutionViewModel>>(baseUrl + "/api/pollution?zip=" + zip);
			return result;
		}
	}
}