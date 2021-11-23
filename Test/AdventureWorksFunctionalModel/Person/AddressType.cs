namespace AW.Types;

[Bounded]
public class AddressType : IHasModifiedDate, IHasRowGuid
{
    [Hidden]
    public int AddressTypeID { get; init; }

    [Hidden]
    public string Name { get; init; } = "";


    [Hidden]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Name;
}