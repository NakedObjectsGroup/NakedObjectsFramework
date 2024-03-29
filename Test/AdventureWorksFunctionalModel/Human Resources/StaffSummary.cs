﻿using AW.Functions;

namespace AW.Types;

[ViewModel(typeof(StaffSummary_Functions))]
public class StaffSummary
{
    [MemberOrder(1)]
    public int Female { get; init; }

    [MemberOrder(2)]
    public int Male { get; init; }

    [MemberOrder(3)]
    public int TotalStaff => Female + Male;

    public override string ToString() => "Staff Summary";
}