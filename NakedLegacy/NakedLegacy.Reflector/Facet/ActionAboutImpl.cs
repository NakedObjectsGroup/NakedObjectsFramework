namespace NakedLegacy.Reflector.Facet;

public class ActionAboutImpl : ActionAbout {
    public ActionAboutImpl(AboutTypeCodes typeCode) => TypeCode = typeCode;

    public string Name { get; set; }
    public string Description { get; set; }
    public bool Visible { get; set; } = true;
    public bool Usable { get; set; } = true;
    public string UnusableReason { get; set; }
    public bool[] ParamsRequired { get; set; }
    public string[] ParamLabels { get; set; }
    public object[] ParamDefaultValues { get; set; }
    public object[][] ParamOptions { get; set; }
    public AboutTypeCodes TypeCode { get; }
}