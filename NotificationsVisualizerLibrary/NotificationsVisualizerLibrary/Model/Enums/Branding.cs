using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    [ObjectModelEnum("TileBranding", NotificationType.Tile)]
    internal enum Branding
    {
        None,
        Name,
        Logo,
        NameAndLogo
    }
}
