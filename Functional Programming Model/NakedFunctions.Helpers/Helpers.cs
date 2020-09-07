using System;

namespace NakedFunctions
{
    public static class Helpers
    {
        public static (T, T) DisplayAndPersist<T>(T obj)
        {
            return (obj, obj);
        }

        public static Action<IUserAdvisory> WarnUser(string message)
        {
            return (IUserAdvisory ua) => ua.WarnUser(message);
        }

        public static Action<IUserAdvisory> InformUser(string message)
        {
            return (IUserAdvisory ua) => ua.InformUser(message);
        }
    }
}
