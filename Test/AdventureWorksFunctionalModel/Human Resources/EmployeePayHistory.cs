namespace AW.Types;

public class EmployeePayHistory
{
    [Hidden]
    public int EmployeeID { get; init; }

    [MemberOrder(1), Mask("d")]
    public DateTime RateChangeDate { get; init; }

    [MemberOrder(2), Mask("C")]
    public decimal Rate { get; init; }

    [MemberOrder(3)]
    public byte PayFrequency { get; init; }

    [MemberOrder(4)]
    public virtual Employee Employee { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"{Rate.ToString("C")} from {RateChangeDate.ToString("d")}";
}