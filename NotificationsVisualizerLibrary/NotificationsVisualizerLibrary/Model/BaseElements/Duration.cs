using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelEnum("ToastDuration", NotificationType.Toast)]
    internal enum Duration
    {
        Short,
        Long
    }
}
