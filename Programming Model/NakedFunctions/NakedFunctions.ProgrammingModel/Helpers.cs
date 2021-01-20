namespace NakedFunctions
{
    public static class Helpers
    {
        public static (T, IContext) SaveAndDisplay<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));
    }
}
