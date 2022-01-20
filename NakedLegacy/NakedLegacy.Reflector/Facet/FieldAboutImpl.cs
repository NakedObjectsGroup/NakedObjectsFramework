namespace NakedLegacy.Reflector.Facet;

public class FieldAboutImpl : FieldAbout {
    public FieldAboutImpl(AboutTypeCodes typeCode) => TypeCode = typeCode;

    public bool Required { get; set; }
    public object DefaultValue { get; set; }

    public AboutTypeCodes TypeCode { get; }

    public bool Usable { get; set; } = true;
    public bool Visible { get; set; } = true;

    public bool IsPersistent { get; set; }
    public object[] Options { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string UnusableReason { get; set; }

    public bool IsValid { get; set; } = true;
    public string InvalidReason { get; set; }
}