namespace AW.Types;

[Bounded]
public class PhoneNumberType : IHasModifiedDate
{
    [Hidden]
    public int PhoneNumberTypeID { get; init; }

    [Hidden]
    public string? Name { get; init; }

    [Hidden, Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string? ToString() => Name;
}