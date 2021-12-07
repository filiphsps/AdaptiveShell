using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;

namespace NotificationsVisualizerLibrary.Manifest
{
    [XmlRoot(ElementName = "Package", Namespace = "http://schemas.microsoft.com/appx/2010/manifest")]
    public sealed class Package
    {
        [XmlIgnore]
        public string RootPath { get; set; }

        [XmlElement]
        public Applications Applications { get; set; }


        public IAsyncOperation<string> GetMrtUri(string path)
        {
            return Task.Run(async delegate
            {
                var fullLogoPath = Path.Combine(RootPath, path);

                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(fullLogoPath);

                foreach (StorageFile file in await folder.GetFilesAsync())
                {
                    if (file.Name.StartsWith(Path.GetFileNameWithoutExtension(path)) && file.Name.EndsWith(Path.GetExtension(path)))
                        return file.Path;
                }

                return null;
            }).AsAsyncOperation();
        }
    }
}
