namespace AW.Types;

[Bounded]
public class Department : IHasModifiedDate
{
    [Hidden]
    public short DepartmentID { get; init; }

    [MemberOrder(1)]
    public string Name { get; init; } = "";

    [MemberOrder(2)]
    public string GroupName { get; init; } = "";

    [MemberOrder(99),Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}