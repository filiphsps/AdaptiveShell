using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelEnum("ToastScenario")]
    internal enum Scenario
    {
        Default,
        Alarm,
        Reminder,
        IncomingCall
    }
}
