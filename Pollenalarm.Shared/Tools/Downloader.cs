using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace Pollenalarm.Shared
{
	public static class Downloader
	{
		/// <summary>
		/// Downloads JSON and converts it to a specific model
		/// </summary>
		/// <param name="url">URL.</param>
		public static async Task<T> DownloadAsync<T>(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			try
			{
				var response = await request.GetResponseAsync();
				var receiveStream = response.GetResponseStream();
				var readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
				var content = await readStream.ReadToEndAsync();

				var result = JsonConvert.DeserializeObject<T>(content);
				return result;
			}
			catch (Exception)
			{
				return default(T);
			}
		}
	}
}