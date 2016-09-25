using System;
using System.Threading.Tasks;

namespace Pollenalarm.Core
{
	public interface IHttpService
	{
		Task<string> GetStringAsync(string url, TimeSpan? cacheTime = null, TimeSpan? timeout = null);
	}
}
