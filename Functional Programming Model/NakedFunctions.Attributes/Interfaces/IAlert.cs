namespace NakedFunctions
{
    /// <summary>
    /// Defines a service for which the framework can provide an implementation, allowing messages to be displayed on the user interface.
    /// </summary>
    public interface IAlert
    {
        void WarnUser(string message);
        void InformUser(string message);
    }
}
