namespace SibCCSPETest.Shared.Components
{
    public interface INexusNotificationService
    {
        event Action<NexusNotificationLevel, string, string> OnShow;
        void ShowInfo(string message, string heading = "");
        void ShowSuccess(string message, string heading = "");
        void ShowWarning(string message, string heading = "");
        void ShowError(string message, string heading = "");
        void ShowNotification(NexusNotificationLevel level, string message, string heading = "");
    }
}
