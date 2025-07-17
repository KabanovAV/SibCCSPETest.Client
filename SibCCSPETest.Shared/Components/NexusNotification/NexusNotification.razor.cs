using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusNotification
    {
        [CascadingParameter]
        private NexusNotificationHub Hub { get; set; }

        [Parameter]
        public NexusNotificationInstance Instance { get; set; }

        private void Close()
            => Hub.RemoveNotification(Instance.Id);
    }
}
