using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace NotificationsVisualizerLibrary.Renderers
{
    internal static class ImageHelper
    {
        internal static BitmapImage GetBitmap(string uriStr)
        {
            if (uriStr == null)
                return null;

            try
            {
                if (uriStr.StartsWith("ms-appdata", StringComparison.CurrentCultureIgnoreCase))
                {
                    BitmapImage img = new BitmapImage();

                    // I'll set the image after first returning
                    var dontWait = SetAppDataImageAsync(img, uriStr);

                    return img;
                }


                return new BitmapImage(GetImageUri(uriStr))
                {
                    
                };
            }

            catch { return null; }
        }

        private static async Task SetAppDataImageAsync(BitmapImage img, string uriStr)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uriStr));

                using (Stream s = await file.OpenStreamForReadAsync())
                {
                    MemoryStream copiedStream = new MemoryStream((int)s.Length);

                    s.CopyTo(copiedStream);

                    copiedStream.Position = 0;

                    try
                    {
                        img.SetSource(copiedStream.AsRandomAccessStream());
                    }

                    catch
                    {
                        copiedStream.Dispose();
                    }
                }
            }

            catch (Exception ex)
            {

            }
        }

        internal static Uri GetImageUri(string uriStr)
        {
            var uri = new Uri("image1.png", UriKind.Relative);

            if (!Uri.TryCreate(uriStr, UriKind.Absolute, out uri))
            {
                try
                {
                    uri = new Uri(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, uriStr), UriKind.Absolute);
                }
                catch (Exception)
                {
                    Debug.WriteLine("INVALID IMAGE URI: {0}", uriStr);
                }
            }

            return uri;
        }
    }
}
