using System;

namespace NakedFunctions
{
    public static class Helpers
    {
        public static (T, T) DisplayAndPersist<T>(T obj)
        {
            return (obj, obj);
        }

        public static Action<IAlert> WarnUser(string message)
        {
            return (IAlert ua) => ua.WarnUser(message);
        }

        public static Action<IAlert> InformUser(string message)
        {
            return (IAlert ua) => ua.InformUser(message);
        }
    }
}
