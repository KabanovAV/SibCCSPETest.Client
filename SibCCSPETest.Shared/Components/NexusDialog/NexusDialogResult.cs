namespace SibCCSPETest.Shared.Components
{
    public class NexusDialogResult
    {
        public object? Data { get; }
        public Type? DataType { get; }
        public bool Canceled { get; }

        protected internal NexusDialogResult(object? data, Type? resultType, bool canceled)
        {
            Data = data;
            DataType = resultType;
            Canceled = canceled;
        }

        public static NexusDialogResult Ok<T>(T result) => Ok(result, default);
        public static NexusDialogResult Ok<T>(T result, Type? dialogType) => new(result, dialogType, false);
        public static NexusDialogResult Cancel() => new(default, typeof(object), true);
    }
}
