namespace AW.Types;

public class BillOfMaterial : IHasModifiedDate
{
    [Hidden]
    public int BillOfMaterialID { get; init; }

    public DateTime StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public short BOMLevel { get; init; }

    public decimal PerAssemblyQty { get; init; }

    [Hidden]
    public int? ProductAssemblyID { get; init; }

    public virtual Product? Product { get; init; }

    [Hidden]
    public int ComponentID { get; init; }

    public virtual Product Product1 { get; init; }

    [Hidden]
    public string UnitMeasureCode { get; init; } = "";

    public virtual UnitMeasure UnitMeasure { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"BillOfMaterial: {BillOfMaterialID}";
}