namespace AW.Types;

public class SalesTaxRate
{
    [Hidden]
    public int SalesTaxRateID { get; init; }

    public byte TaxType { get; init; }

    public decimal TaxRate { get; init; }

    public string Name { get; init; } = "";

    [Hidden]
    public int StateProvinceID { get; init; }

    public virtual StateProvince StateProvince { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public Guid rowguid { get; init; }

    public override string ToString() => $"Sales Tax Rate for {StateProvince}";
}