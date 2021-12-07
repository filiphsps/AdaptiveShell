using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelEnum("ToastActivationType", NotificationType.Toast)]
    internal enum ActivationType
    {
        None,
        Foreground,
        Background,
        Protocol,
        System
    }
}
