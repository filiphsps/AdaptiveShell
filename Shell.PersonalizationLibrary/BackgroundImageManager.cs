using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Shell.PersonalizationLibrary {
    public class BackgroundImageManager {
        public static async Task<BitmapImage> GetBackgroundImage() {
            try {
                StorageFolder path = await StorageFolder.GetFolderFromPathAsync(
                    Path.Combine(ApplicationData.Current.LocalFolder.Path.Split("\\Local")[0], "Roaming\\Microsoft\\Windows\\Themes\\CachedFiles")
                );

                StorageFile background = (await path.GetFilesAsync()).FirstOrDefault();

                using (IRandomAccessStream fileStream = await background.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                    // Set the image source to the selected bitmap
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
            } catch {
                return null;
            }
        }
    }
}
