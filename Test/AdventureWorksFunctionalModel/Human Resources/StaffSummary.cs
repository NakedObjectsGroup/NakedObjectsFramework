using NakedFunctions;
using static AW.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AW.Types
{

    [ViewModel]
    public record StaffSummary
    {
        [MemberOrder(1)]
        public virtual int Female { get; init; }

        [MemberOrder(2)]
        public virtual int Male { get; init; }

        [MemberOrder(3)]
        public virtual int TotalStaff { get; init; }

        public override string ToString() => "Staff Summary";
    }

}
