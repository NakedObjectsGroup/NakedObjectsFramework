using NakedFunctions;
using AW.Types;
using static AW.Helpers;
using System;

namespace AW.Functions
{
    public static class WorkOrderRoutingFunctions
    {
        public static string ValidatePlannedCost(this WorkOrderRouting wor, decimal plannedCost)
        {
            return plannedCost <= 0 ? "Planned cost must be > 0" : "";
        }

        [MemberOrder(1)]
        public static (WorkOrderRouting, IContext) SetScheduledStartDate(this WorkOrderRouting wor, DateTime date, int hour, int minutes, IContext context)
            => DisplayAndSave(wor with { ScheduledStartDate = date.AddHours(hour).AddMinutes(minutes) }, context);


        [MemberOrder(2)]
        public static (WorkOrderRouting, IContext) SetScheduledEndDate(this WorkOrderRouting wor, DateTime date, [Optionally] int hour, [Optionally] int minutes, IContext context)
        => DisplayAndSave(wor with { ScheduledEndDate = date.AddHours(hour).AddMinutes(minutes) }, context);
    }
}
