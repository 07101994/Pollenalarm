using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class PollenServiceAzure : PollenService
    {
        private MobileServiceClient _MobileClient;
        private IMobileServiceSyncTable<Pollen> _PollenTable;

        public PollenServiceAzure(SettingsService settingsService) : base(settingsService)
        {
            _MobileClient = new MobileServiceClient("");
        }

        private async Task InitializeAsync()
        {
            // Check, if synchronization has already been initialized, as we don't want to do this more than once
            if (_MobileClient?.SyncContext?.IsInitialized ?? false)
                return;

            // Setup the local SQLite database
            var path = Path.Combine(MobileServiceClient.DefaultDatabasePath, "syncstore.db");
            var store = new MobileServiceSQLiteStore(path);

            // Define a pendant to the remote tables at the local database to sync with
            store.DefineTable<Pollen>();

            // Tell the client to synchronize its tables against this database
            await _MobileClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            // Get tables from the server
            _PollenTable = _MobileClient.GetSyncTable<Pollen>();
        }

        public override async Task<List<Pollen>> GetAllPollenAsync()
        {
            // Initialize local sync store if needed
            await InitializeAsync();

            // Sync changes to local store
            await SyncAsync();

            // Return form local store
            return await _PollenTable.ToListAsync();
        }

        public override async Task<bool> GetPollutionsForPlaceAsync(Place place)
        {
            throw new NotImplementedException();
        }

        private async Task SyncAsync()
        {
            try
            {
                // Get changed datasets from remote and merge them
                await _PollenTable.PullAsync("allPollen", _PollenTable.CreateQuery());

                // Push local changes back to the remote
                await _MobileClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                // Unable to sync speakers, that is alright as we have offline capabilities
                Debug.WriteLine("Unable to sync with backend." + ex);
            }
        }
    }
}
