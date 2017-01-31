using System.Threading.Tasks;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
	public class SettingsService
	{
		private IFileSystemService _FileSystemService;
		private bool _IsLoaded;

		public Settings CurrentSettings { get; set; }

		public SettingsService(IFileSystemService fileSystemService)
		{
			_FileSystemService = fileSystemService;
		}

		public async Task SaveSettingsAsync()
		{
			await _FileSystemService.SaveObjectToFileAsync("settings.json", CurrentSettings);
		}

		public async Task LoadSettingsAsync()
		{
			if (!_IsLoaded)
			{
				var settings = await _FileSystemService.ReadObjectFromFileAsync<Settings>("settings.json");
				if (settings != null)
					CurrentSettings = settings;
				else
					CurrentSettings = new Settings();

				_IsLoaded = true;
			}
		}
	}
}
