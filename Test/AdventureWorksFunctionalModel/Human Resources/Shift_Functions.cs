






using System;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class ShiftFunctions {
        internal static IContext UpdateShift(Shift original, Shift updated, IContext context) =>
            context.WithUpdated(original, new(updated) { ModifiedDate = context.Now() });

        [Edit]
        public static IContext EditTimes(this Shift s,
                                         TimeSpan startTime, TimeSpan endTime, IContext context) =>
            UpdateShift(s, new(s) { StartTime = startTime, EndTime = endTime }, context);

        public static string? ValidateEditTimes(
            this Shift s, TimeSpan startTime, TimeSpan endTime) =>
            endTime > startTime ? null : "End time must be after start time";

        [Edit]
        public static IContext EditName(this Shift s,
                                        string name, IContext context) =>
            UpdateShift(s, new(s) { Name = name }, context);
    }
}