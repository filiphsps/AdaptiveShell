using NotificationsVisualizerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Shell.Models {
    public class TileDataModel {
        public String AppId;
        public XmlDocument Payload;
    }

    public class TileModel {
        public String AppId { get => this.Entry.AppUserModelId; }
        public String DisplayName { get => this.LiveTile.DisplayName; set => this.LiveTile.DisplayName = value; }
        public Boolean IsPinned { get; set; }
        public String Publisher { get; set; }
        public TileSize Size { get => this.LiveTile.TileSize; set => this.LiveTile.TileSize = value; }
        public PreviewTile LiveTile { get; set; }
        public List<TileDataModel> TileData { get; set; }

        public Int32 RowSpan {
            get {
                switch (this.LiveTile.TileSize) {
                    case TileSize.Large:
                        return 4;
                    case TileSize.Wide:
                    case TileSize.Medium:
                        return 2;
                    case TileSize.Small:
                    default:
                        return 1;
                }
            }
        }

        public Int32 ColumnSpan {
            get {
                switch (this.LiveTile.TileSize) {
                    case TileSize.Large:
                        return 4;
                    case TileSize.Wide:
                        return 4;
                    case TileSize.Medium:
                        return 2;
                    case TileSize.Small:
                    default:
                        return 1;
                }
            }
        }

        public ImageBrush Logo { get; set; }
        public Package Package { get; set; }
        public AppListEntry Entry { get; set; }
    }
}
