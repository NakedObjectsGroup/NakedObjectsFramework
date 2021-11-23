namespace AW.Types;

public class Password : IHasRowGuid, IHasModifiedDate
{
    public Password() { }

    public Password(Password cloneFrom)
    {
        BusinessEntityID = cloneFrom.BusinessEntityID;
        Person = cloneFrom.Person;
        PasswordHash = cloneFrom.PasswordHash;
        PasswordSalt = cloneFrom.PasswordSalt;
        ModifiedDate = cloneFrom.ModifiedDate;
        rowguid = cloneFrom.rowguid;
    }

    [Hidden]
    public int BusinessEntityID { get; init; }

    [Hidden]
    public virtual Person Person { get; init; }

    [Hidden]
    public string PasswordHash { get; init; } = "";

    [Hidden]
    public string PasswordSalt { get; init; } = "";

    [Hidden, Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => "Password";
}