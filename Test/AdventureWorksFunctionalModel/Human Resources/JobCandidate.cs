namespace AW.Types;

public class JobCandidate
{
    [Hidden]
    public int JobCandidateID { get; init; }

    public string? Resume { get; init; }

    [Hidden]
    public int? EmployeeID { get; init; }

    public virtual Employee? Employee { get; init; }

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => "Job Candidate ";
}