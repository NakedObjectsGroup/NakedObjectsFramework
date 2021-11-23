namespace AW.Types;

[Bounded]
public class Shift : IHasModifiedDate
{
    public Shift() { }

    public Shift(Shift cloneFrom)
    {
        ShiftID = cloneFrom.ShiftID;
        Name = cloneFrom.Name;
        StartTime = cloneFrom.StartTime;
        EndTime = cloneFrom.EndTime;
        ModifiedDate = cloneFrom.ModifiedDate;
    }

    [Hidden]
    public byte ShiftID { get; init; }

    [MemberOrder(1)]
    public string Name { get; init; } = "";

    [MemberOrder(3), Mask("T")]
    public TimeSpan StartTime { get; init; }

    [MemberOrder(4), Mask("T")]
    public TimeSpan EndTime { get; init; }


    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}