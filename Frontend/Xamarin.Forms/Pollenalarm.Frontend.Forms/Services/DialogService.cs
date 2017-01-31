using Pollenalarm.Frontend.Shared.Services;
using System.Threading.Tasks;
using System;

namespace Pollenalarm.Frontend.Forms.Services
{
	public class DialogService : IDialogService
	{
		public async Task DisplayAlertAsync(string title, string message, string ok)
		{
			await App.Current.MainPage.DisplayAlert(title, message, ok);
		}

		public async Task<bool> DisplayConfirmationAsync(string title, string message, string confirm, string cancel)
		{
			return await App.Current.MainPage.DisplayAlert(title, message, confirm, cancel);
		}
	}
}