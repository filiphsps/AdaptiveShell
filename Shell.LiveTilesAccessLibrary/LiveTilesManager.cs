using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace Shell.LiveTilesAccessLibrary {
    public class LiveTilesManager {
        public LiveTilesManager() {
            this.Path = ApplicationData.Current.LocalFolder.Path.Split("\\Packages")[0];
        }

        private String Path;
        public async Task<List<TileModel>> GetLiveTiles() {
            // Recover in case of previous unexpected termination.
            await this.Cleanup();

            // You'll need to enable file system access!
            StorageFolder location = await StorageFolder.GetFolderFromPathAsync($"{this.Path}\\Microsoft\\Windows\\Notifications\\");
            StorageFile dbFile = await this.CreateTempFiles(location);

            Debug.WriteLine(dbFile.Path);

            var tiles = new List<TileModel>();
            try {
                using (var db = new SqliteConnection($"Filename={dbFile.Path}")) {
                    await db.OpenAsync();

                    // Enable write-ahead logging
                    SqliteCommand walCommand = db.CreateCommand();
                    walCommand.CommandText = "PRAGMA wal_checkpoint";
                    walCommand.ExecuteNonQuery();

                    SqliteDataReader tileQuery = (new SqliteCommand("SELECT * from Notification WHERE Type=\"tile\"", db)).ExecuteReader();
                    while (tileQuery.Read()) {
                        String handle = tileQuery.GetString(2);
                        SqliteDataReader handleQuery = new SqliteCommand($"SELECT * from NotificationHandler WHERE RecordId=\"{handle}\"", db).ExecuteReader();
                        handleQuery.Read();

                        tiles.Add(new TileModel {
                            Payload = tileQuery.GetString(5),
                            AppId = handleQuery.GetString(1)
                        });
                    }

                    db.Close();
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }

            await this.Cleanup();
            return tiles;
        }

        private async Task<StorageFile> CreateTempFiles(StorageFolder location) {
            await (await location.GetFileAsync("wpndatabase.db-shm")).CopyAsync(ApplicationData.Current.LocalFolder, "wpndatabase.db-shm");
            await (await location.GetFileAsync("wpndatabase.db-wal")).CopyAsync(ApplicationData.Current.LocalFolder, "wpndatabase.db-wal");
            return await (await location.GetFileAsync("wpndatabase.db")).CopyAsync(ApplicationData.Current.LocalFolder, "wpndatabase.db");
        }

        private async Task Cleanup() {
            StorageFolder location = ApplicationData.Current.LocalFolder;

            try {
                await (await location.GetFileAsync("wpndatabase.db-shm")).DeleteAsync();
            } catch { }
            try {
                await (await location.GetFileAsync("wpndatabase.db-wal")).DeleteAsync();
            } catch { }
            try {
                await (await location.GetFileAsync("wpndatabase.db")).DeleteAsync();
            } catch { }
        }
    }
}
