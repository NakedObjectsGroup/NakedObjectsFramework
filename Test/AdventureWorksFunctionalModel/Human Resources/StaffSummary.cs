﻿using AW.Functions;
using NakedFunctions;

namespace AW.Types
{

    [ViewModel(typeof(StaffSummary_Functions))]
    public record StaffSummary
    {
        [MemberOrder(1)]
        public virtual int Female { get; init; }

        [MemberOrder(2)]
        public virtual int Male { get; init; }

        [MemberOrder(3)]
        public virtual int TotalStaff => Female + Male;

        public override string ToString() => "Staff Summary";

        public virtual bool Equals(StaffSummary other) => ReferenceEquals(this, other);

        public override int GetHashCode() => base.GetHashCode();
    }

}
