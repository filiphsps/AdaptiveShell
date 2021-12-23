using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.Enums {
    [ObjectModelEnum("TileBranding", XmlTemplateParser.NotificationType.Tile)]
    public enum Branding {
        None,
        Name,
        Logo,
        NameAndLogo
    }
}
