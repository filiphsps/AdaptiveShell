using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal interface IActivatableElement
    {
        string Arguments { get; set; }

        ActivationType ActivationType { get; set; }
    }
}
