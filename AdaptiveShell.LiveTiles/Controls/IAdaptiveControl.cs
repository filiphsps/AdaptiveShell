using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Controls {
    public interface IAdaptiveControl {
        Boolean DoesAllContentFit { get; }
    }
}
