namespace SibCCSPETest.Shared.Components
{
    public class NexusDialogService : INexusDialogService
    {
        public event Action<NexusDialogReference, NexusDialogSetting> OnShow;
        public event Action<Guid, NexusDialogResult?>? OnDialogCloseRequested;

        public async Task<NexusDialogResult?> Show(NexusDialogSetting setting)
        {
            var dialogReference = new NexusDialogReference();
            OnShow?.Invoke(dialogReference, setting);
            return await dialogReference.Result;
        }

        public void Close(Guid dialogId, NexusDialogResult? result)
        {
            OnDialogCloseRequested?.Invoke(dialogId, result);
        }
    }
}
