namespace AW.Types;

public class EmailAddress : IHasRowGuid, IHasModifiedDate
{
    [Hidden]
    public int BusinessEntityID { get; init; }

    [Hidden]
    public int EmailAddressID { get; init; }

    [Named("Email Address")]
    public string? EmailAddress1 { get; init; }


    [Hidden, Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string? ToString() => EmailAddress1;
}