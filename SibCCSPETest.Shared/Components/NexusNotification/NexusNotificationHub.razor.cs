using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusNotificationHub
    {
        [Inject]
        private INexusNotificationService _notificationService { get; set; }

        [Parameter]
        public string InfoClass { get; set; }
        [Parameter]
        public string InfoIconClass { get; set; }
        [Parameter]
        public string SuccessClass { get; set; }
        [Parameter]
        public string SuccessIconClass { get; set; }
        [Parameter]
        public string WarningClass { get; set; }
        [Parameter]
        public string WarningIconClass { get; set; }
        [Parameter]
        public string ErrorClass { get; set; }
        [Parameter]
        public string ErrorIconClass { get; set; }
        [Parameter]
        public NexusNotificationPosition Position { get; set; } = NexusNotificationPosition.TopRight;
        [Parameter]
        public int Timeout { get; set; } = 5;

        private string PositionClass { get; set; } = string.Empty;
        internal List<NexusNotificationInstance> NotificationItems { get; set; } = [];

        protected override void OnInitialized()
        {
            _notificationService.OnShow += ShowNotification;
            PositionClass = $"position-{Position.ToString().ToLower()}";
        }

        private void ShowNotification(NexusNotificationLevel level, string message, string heading)
        {
            InvokeAsync(() =>
            {
                var settings = BuildNotificationSettings(level, message, heading);
                var notification = new NexusNotificationInstance
                {
                    Id = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    Setting = settings
                };

                NotificationItems.Add(notification);

                var timeout = Timeout * 1000;
                var notificationTimer = new System.Timers.Timer(timeout);
                notificationTimer.Elapsed += (sender, args) => { RemoveNotification(notification.Id); };
                notificationTimer.AutoReset = false;
                notificationTimer.Start();

                StateHasChanged();
            });
        }

        private NexusNotificationSetting BuildNotificationSettings(NexusNotificationLevel level, string message, string heading)
        {
            switch (level)
            {
                case NexusNotificationLevel.Info:
                    return new NexusNotificationSetting(string.IsNullOrWhiteSpace(heading) ? "Info" : heading, message, "nexus-notification-info", InfoClass, InfoIconClass);
                case NexusNotificationLevel.Success:
                    return new NexusNotificationSetting(string.IsNullOrWhiteSpace(heading) ? "Success" : heading, message, "nexus-notification-success", SuccessClass, SuccessIconClass);
                case NexusNotificationLevel.Warning:
                    return new NexusNotificationSetting(string.IsNullOrWhiteSpace(heading) ? "Warning" : heading, message, "nexus-notification-warning", WarningClass, WarningIconClass);
                case NexusNotificationLevel.Error:
                    return new NexusNotificationSetting(string.IsNullOrWhiteSpace(heading) ? "Error" : heading, message, "nexus-notification-error", ErrorClass, ErrorIconClass);
            }
            throw new InvalidOperationException();
        }

        public void RemoveNotification(Guid toastId)
        {
            InvokeAsync(() =>
            {
                var notificationInstance = NotificationItems.SingleOrDefault(x => x.Id == toastId);
                NotificationItems.Remove(notificationInstance);
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            _notificationService.OnShow -= ShowNotification;
        }
    }
}
