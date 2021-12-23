using AdaptiveShell.LiveTiles.Models.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.Enums {
    [ObjectModelEnum("AdaptiveTextStyle")]
    internal enum HintStyle {
        Default,
        Caption,
        Body,
        Base,
        Subtitle,
        Title,
        Subheader,
        Header,
        CaptionSubtle,
        BodySubtle,
        BaseSubtle,
        SubtitleSubtle,
        TitleSubtle,
        SubheaderSubtle,
        HeaderSubtle,
        SubheaderNumeral,
        SubheaderNumeralSubtle,
        HeaderNumeral,
        HeaderNumeralSubtle,
        TitleNumeral,
        TitleNumeralSubtle
    }
}
