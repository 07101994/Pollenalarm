using System.Globalization;

namespace Pollenalarm.Frontend.Forms.Services
{
	public interface ICultureService
	{
		CultureInfo GetCurrentCultureInfo();
	}
}
