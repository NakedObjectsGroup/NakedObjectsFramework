namespace AW.Functions;
public static class ShiftFunctions
{
    [Edit]
    public static IContext EditTimes(this Shift s,
                                     TimeSpan startTime, TimeSpan endTime, IContext context) =>
        context.WithUpdated(s, new(s)
        {
            StartTime = startTime,
            EndTime = endTime,
            ModifiedDate = context.Now()
        });

    public static string? ValidateEditTimes(
        this Shift s, TimeSpan startTime, TimeSpan endTime) =>
        endTime > startTime ? null : "End time must be after start time";

    [Edit]
    public static IContext EditName(this Shift s,
                                    string name, IContext context) =>
        context.WithUpdated(s, new(s)
        {
            Name = name,
            ModifiedDate = context.Now()
        });
}