using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    [ViewModel(typeof(StaffSummary_Functions))]
    public record StaffSummary {
        [MemberOrder(1)]
        public int Female { get; init; }

        [MemberOrder(2)]
        public int Male { get; init; }

        [MemberOrder(3)]
        public int TotalStaff => Female + Male;

        public virtual bool Equals(StaffSummary? other) => ReferenceEquals(this, other);

        public override string ToString() => "Staff Summary";

        public override int GetHashCode() => base.GetHashCode();
    }
}