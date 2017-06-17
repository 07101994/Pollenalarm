using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Pollenalarm.Core;

namespace Pollenalarm.Frontend.Forms
{
	public class HttpService : IHttpService
	{
		private HttpClient _HttpClient;

		public HttpService()
		{
			_HttpClient = new HttpClient(new NativeMessageHandler());
		}

		public async Task<string> GetStringAsync(string url, TimeSpan? cacheTime = default(TimeSpan?), TimeSpan? timeout = default(TimeSpan?))
		{
			try
			{
				var result = await _HttpClient.GetStringAsync(url);
				return result;
			}
			catch (HttpRequestException)
			{
				return null;
			}
		}
	}
}
