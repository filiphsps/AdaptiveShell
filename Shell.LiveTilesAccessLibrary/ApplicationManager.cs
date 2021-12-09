using Microsoft.Data.Sqlite;
using NotificationsVisualizerLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Shell.LiveTilesAccessLibrary {
    public class ApplicationManager {
        public ObservableCollection<TileModel> LiveTiles { get; set; } = new ObservableCollection<TileModel>();

        private String NotificationsPath;

        public ApplicationManager() {
            this.NotificationsPath = $"{ApplicationData.Current.LocalFolder.Path.Split("\\Packages")[0]}\\Microsoft\\Windows\\Notifications\\";
        }

        public async Task Initilize() {
            await this.Update();
        }

        public async Task<ObservableCollection<TileModel>> Update() {
            // Get all packages.
            var packageManager = new PackageManager();
            var packages = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();

            // Get the new live tile data.
            var tilesData = await this.GetLiveTilesData();

            // Remove all live tiles.
            this.LiveTiles.Clear();

            // Create a live tile for each entry.
            foreach (Package package in packages) {
                if (package.DisplayName.Length <= 0)
                    continue;

                if (package.IsFramework || package.IsResourcePackage)
                    continue;

                foreach (AppListEntry entry in await package.GetAppListEntriesAsync()) {
                    if (entry.DisplayInfo.DisplayName.Length <= 0)
                        continue;
                    if (entry.DisplayInfo.DisplayName == "NoUIEntryPoints-DesignMode")
                        continue;

                    var imageStream = await entry.DisplayInfo.GetLogo(new Windows.Foundation.Size(250, 250)).OpenReadAsync();
                    var memStream = new MemoryStream();
                    await imageStream.AsStreamForRead().CopyToAsync(memStream);
                    memStream.Position = 0;
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(memStream.AsRandomAccessStream());

                    var logo = new ImageBrush() {
                        ImageSource = bitmap,
                        Stretch = Stretch.UniformToFill
                    };

                    var tile = new TileModel {
                        TileData = tilesData.FindAll(i => i.AppId == entry.AppUserModelId),
                        LiveTile = new PreviewTile() {
                            DisplayName = entry.DisplayInfo.DisplayName,
                            TileSize = TileSize.Medium,
                            TileDensity = TileDensity.Mobile(1.75),
                            VisualElements = {
                                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                                ShowNameOnSquare150x150Logo = true,
                                ShowNameOnSquare310x310Logo = true,
                                ShowNameOnWide310x150Logo = true
                            }
                        },
                        Package = package,
                        Entry = entry,
                        Logo = new ImageBrush() {
                            ImageSource = bitmap,
                            Stretch = Stretch.UniformToFill
                        }
                    };

                    await tile.LiveTile.UpdateAsync();
                    this.LiveTiles.Add(tile);
                }
            }

            return this.LiveTiles;
        }


        private async Task<List<TileDataModel>> GetLiveTilesData() {
            // Recover in case of previous unexpected termination.
            await this.Cleanup();

            // You'll need to enable file system access!
            StorageFolder location = await StorageFolder.GetFolderFromPathAsync(this.NotificationsPath);
            StorageFile dbFile = await this.CreateTempFiles(location);

            var tiles = new List<TileDataModel>();
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

                        var payload = new XmlDocument();
                        payload.LoadXml(tileQuery.GetString(5));
                        payload.Normalize();

                        tiles.Add(new TileDataModel {
                            Payload = payload,
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
