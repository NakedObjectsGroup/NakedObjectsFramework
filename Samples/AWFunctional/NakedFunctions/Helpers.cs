using System;

namespace NakedFunctions
{
   public static class Helpers
    {
        public static T Clone<T>(this T obj) where T : IFunctionalType
        { throw new NotImplementedException(); }

        public static Feedback Add(this Feedback addTo, string message, FeedbackType type) { throw new NotImplementedException(); }
    }
}
