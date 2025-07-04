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
