﻿using NakedFunctions;
using static NakedFunctions.Helpers;
using System;

namespace AdventureWorksModel
{
    public static class WorkOrderRoutingFunctions
    {
        public static string ValidatePlannedCost(this WorkOrderRouting wor, decimal plannedCost)
        {
            return plannedCost <= 0 ? "Planned cost must be > 0" : "";
        }

        [MemberOrder(1)]
        public static (WorkOrderRouting, WorkOrderRouting) SetScheduledStartDate(this WorkOrderRouting wor, DateTime date, int hour, int minutes)
            => DisplayAndPersist(wor with { ScheduledStartDate = date.AddHours(hour).AddMinutes(minutes) });


        [MemberOrder(2)]
        public static (WorkOrderRouting, WorkOrderRouting) SetScheduledEndDate(this WorkOrderRouting wor, DateTime date, [Optionally] int hour, [Optionally] int minutes)
        => DisplayAndPersist(wor with { ScheduledEndDate = date.AddHours(hour).AddMinutes(minutes) });
    }
}
