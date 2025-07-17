namespace SibCCSPETest.Shared.Components
{
    public class NexusNotificationService : INexusNotificationService
    {
        public NexusNotificationService() { }

        public event Action<NexusNotificationLevel, string, string> OnShow;

        public void ShowError(string message, string heading = "")
            => ShowNotification(NexusNotificationLevel.Error, message, heading);

        public void ShowInfo(string message, string heading = "")
            => ShowNotification(NexusNotificationLevel.Info, message, heading);

        public void ShowSuccess(string message, string heading = "")
            => ShowNotification(NexusNotificationLevel.Success, message, heading);

        public void ShowWarning(string message, string heading = "")
            => ShowNotification(NexusNotificationLevel.Warning, message, heading);

        public void ShowNotification(NexusNotificationLevel level, string message, string heading = "")
            => OnShow?.Invoke(level, message, heading);
    }
}
