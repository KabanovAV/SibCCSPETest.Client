namespace SibCCSPETest.Shared.Components
{
    public class NexusDialogInstance
    {
        public Guid Id { get; set; }
        public NexusDialogReference Reference { get; set; }
        public string Show { get; set; }
        public string Display { get; set; }
        public bool IsBackdrop { get; set; }
        public NexusDialogSetting Setting { get; set; }
    }
}
