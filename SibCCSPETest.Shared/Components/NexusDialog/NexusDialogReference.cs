namespace SibCCSPETest.Shared.Components
{
    public class NexusDialogReference
    {
        private readonly TaskCompletionSource<NexusDialogResult?> _resultCompletion = new();
        public Guid Id { get; } = Guid.NewGuid();
        public Task<NexusDialogResult?> Result => _resultCompletion.Task;

        public void Dismiss(NexusDialogResult? result)
        {
            _resultCompletion.TrySetResult(result);
        }
    }
}
