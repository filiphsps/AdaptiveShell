using NotificationsVisualizerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Media;

namespace Shell.LiveTilesAccessLibrary {
    public class TileDataModel {
        public String AppId;
        public XmlDocument Payload;
    }

    public class TileModel {
        public String AppId { get; set; }
        public String DisplayName { get; set; }
        public PreviewTile LiveTile { get; set; }
        public TileSize Size { get; set; }
        public TileDataModel TileData { get; set; }

        public TileDensity Density { get; set; } = TileDensity.Mobile(1.75);

        public Int32 RowSpawn {
            get {
                switch (this.Size) {
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

        public Int32 ColumnSpawn {
            get {
                switch (this.Size) {
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

        public Boolean IsSmall { get => this.Size == TileSize.Small; }
        public Boolean IsMedium { get => this.Size == TileSize.Medium; }
        public Boolean IsWide { get => this.Size == TileSize.Wide; }
        public Boolean IsLarge { get => this.Size == TileSize.Large; }

        public ImageBrush Logo { get; set; }
        public Action Launcher { get; set; }
    }
}
