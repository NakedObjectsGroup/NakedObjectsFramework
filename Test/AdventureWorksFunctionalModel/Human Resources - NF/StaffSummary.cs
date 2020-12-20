using NakedFunctions;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksModel
{

    //Used to test creation/rendering of NotPersisted object
    //Note, however, that recommended pattern would be to use a ViewModel for this
    [NotMapped]
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
