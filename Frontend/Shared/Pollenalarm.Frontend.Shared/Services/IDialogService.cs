using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
	public interface IDialogService
	{
		Task DisplayAlertAsync(string title, string message, string ok);
		Task<bool> DisplayConfirmationAsync(string title, string message, string confirm, string cancel);
	}
}
