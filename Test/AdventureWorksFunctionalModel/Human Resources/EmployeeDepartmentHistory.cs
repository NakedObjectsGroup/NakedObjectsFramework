namespace AW.Types;

public class EmployeeDepartmentHistory
{

    public EmployeeDepartmentHistory() { }

    public EmployeeDepartmentHistory(EmployeeDepartmentHistory cloneFrom)
    {
        EmployeeID = cloneFrom.EmployeeID;
        DepartmentID = cloneFrom.DepartmentID;
        ShiftID = cloneFrom.ShiftID;
        StartDate = cloneFrom.StartDate;
        EndDate = cloneFrom.EndDate;
        Department = cloneFrom.Department;
        Employee = cloneFrom.Employee;
        Shift = cloneFrom.Shift;
        ModifiedDate = cloneFrom.ModifiedDate;
    }

    [Hidden]
    public int EmployeeID { get; init; }

    [Hidden]
    public short DepartmentID { get; init; }

    [Hidden]
    public byte ShiftID { get; init; }

    [MemberOrder(4)]
    [Mask("d")]
    public DateTime StartDate { get; init; }

    [MemberOrder(5)]
    [Mask("d")]
    public DateTime? EndDate { get; init; }

    [MemberOrder(2)]
    public virtual Department Department { get; init; }

    [MemberOrder(1)]
    public virtual Employee Employee { get; init; }

    [MemberOrder(3)]
    public virtual Shift Shift { get; init; }

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"{Department} {StartDate.ToString("d")}";
}
