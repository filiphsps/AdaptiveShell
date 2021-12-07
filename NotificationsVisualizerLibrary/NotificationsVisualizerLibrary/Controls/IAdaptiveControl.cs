using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Controls
{
    public interface IAdaptiveControl
    {
        bool DoesAllContentFit { get; }
    }
}
