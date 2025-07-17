using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusDialog
    {
        [Inject]
        private INexusDialogService DialogService { get; set; }

        private readonly Dictionary<Guid, NexusDialogInstance> _dialogs = new();

        protected override void OnInitialized()
        {
            DialogService.OnShow += ShowDialog;
            DialogService.OnDialogCloseRequested += CloseDialog;
        }

        private void ShowDialog(NexusDialogReference reference, NexusDialogSetting setting)
        {
            InvokeAsync(() =>
            {
                _dialogs[reference.Id] = new NexusDialogInstance
                {
                    Id = reference.Id,
                    Reference = reference,
                    Show = "show",
                    Display = "block",
                    IsBackdrop = true,
                    Setting = setting
                };
                StateHasChanged();
            });
        }

        private void CloseDialog(Guid dialogId, NexusDialogResult? result)
        {
            if (_dialogs.TryGetValue(dialogId, out var instance))
            {
                instance.Reference.Dismiss(result);
                _dialogs.Remove(dialogId);
                StateHasChanged();
            }
        }

        private void CancelDialog(Guid dialogId)
            => DialogService.Close(dialogId, NexusDialogResult.Cancel());

        private void ConfirmDialog(Guid dialogId)
            => DialogService.Close(dialogId, NexusDialogResult.Ok("Данные результата"));

        public void Dispose()
        {
            DialogService.OnShow -= ShowDialog;
            DialogService.OnDialogCloseRequested -= CloseDialog;
        }
    }
}
