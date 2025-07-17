namespace SibCCSPETest.Shared.Components
{
    public class NexusDialogSetting
    {
        public string Heading { get; set; }
        public string Message { get; set; }
        public string CancelButtonTitle { get; set; }
        public string ApplyButtonTitle { get; set; }
        public string BaseClass { get; set; }
        public string AdditionalClasses { get; set; }
        public string IconClass { get; set; }

        public NexusDialogSetting(string heading, string message, string cancelButtonTitle, string applyButtonTitle)
            : this(heading, message, cancelButtonTitle, applyButtonTitle, "", "", "") { }

        public NexusDialogSetting(string heading, string message, string cancelButtonTitle, string applyButtonTitle,
            string baseClass, string additionalClasses, string iconClass)
        {
            Heading = heading;
            Message = message;
            CancelButtonTitle = cancelButtonTitle;
            ApplyButtonTitle = applyButtonTitle;
            BaseClass = baseClass;
            AdditionalClasses = additionalClasses;
            IconClass = iconClass;
        }
    }
}
