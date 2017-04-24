using System.Threading.Tasks;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class SettingsService
    {
        private IFileSystemService _FileSystemService;

        public Settings CurrentSettings { get; set; }

        public SettingsService(IFileSystemService fileSystemService)
        {
            _FileSystemService = fileSystemService;
        }

        public async Task SaveSettingsAsync()
        {
            await _FileSystemService.SaveObjectToFileAsync("settings.json", CurrentSettings);
        }

        /// <summary>
        /// Loads the local settings if needed.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if (CurrentSettings == null)
            {
                var settings = await _FileSystemService.ReadObjectFromFileAsync<Settings>("settings.json");
                if (settings != null)
                    CurrentSettings = settings;
                else
                    CurrentSettings = new Settings();
            }
        }
    }
}
