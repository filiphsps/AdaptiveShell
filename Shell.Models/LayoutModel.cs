using NotificationsVisualizerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Models {
    public class LayoutItemModel {
        public String AppId { get; set; }
        public TileSize Size { get; set; }
    }

    public class LayoutModel {
        public List<LayoutItemModel> Tiles { get; set; }

        public static LayoutModel ToLayoutModel(List<TileModel> tiles) {
            // TODO
            return new LayoutModel {
                Tiles = new List<LayoutItemModel>()
            };
        }
    }
}
