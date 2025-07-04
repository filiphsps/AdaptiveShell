using System;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal interface IActivatableElement
    {
        String Arguments { get; set; }

        ActivationType ActivationType { get; set; }
    }
}
