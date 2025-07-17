namespace SibCCSPETest.Shared.Components
{
    public class NexusNotificationSetting
    {
        public string Heading { get; set; }
        public string Message { get; set; }
        public string BaseClass { get; set; }
        public string AdditionalClasses { get; set; }
        public string IconClass { get; set; }

        public NexusNotificationSetting(string heading, string message, string baseClass, string additionalClasses, string iconClass)
        {
            Heading = heading;
            Message = message;
            BaseClass = baseClass;
            AdditionalClasses = additionalClasses;
            IconClass = iconClass;
        }
    }
}
