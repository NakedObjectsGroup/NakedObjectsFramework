namespace AW.Functions;

public static class WorkOrderRoutingFunctions
{
    public static string ValidatePlannedCost(this WorkOrderRouting wor, decimal plannedCost) => plannedCost <= 0 ? "Planned cost must be > 0" : "";

    [MemberOrder(1)]
    public static IContext SetScheduledStartDate(this WorkOrderRouting wor,
                                                 DateTime date, int hour, int minutes, IContext context) =>
        context.WithUpdated(wor, new(wor)
        {
            ScheduledStartDate = date.AddHours(hour).AddMinutes(minutes),
            ModifiedDate = context.Now()
        });

    [MemberOrder(2)]
    public static IContext SetScheduledEndDate(this WorkOrderRouting wor,
                                               DateTime date, [Optionally] int hour, [Optionally] int minutes, IContext context) =>
        context.WithUpdated(wor, new(wor)
        {
            ScheduledEndDate = date.AddHours(hour).AddMinutes(minutes),
            ModifiedDate = context.Now()
        });
}
