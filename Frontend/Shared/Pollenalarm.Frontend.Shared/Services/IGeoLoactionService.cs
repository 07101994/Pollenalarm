using Pollenalarm.Frontend.Shared.Models;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
	public interface IGeoLoactionService
	{
		Task<GeoLocation> GetCurrentLocationAsync();
	}
}
