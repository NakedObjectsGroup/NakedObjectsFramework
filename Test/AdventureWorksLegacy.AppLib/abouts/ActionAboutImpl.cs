namespace AdventureWorksLegacy.AppLib.abouts;

public class ActionAboutImpl : IActionAbout {
    public ActionAboutImpl(AboutTypeCodes typeCode) => TypeCode = typeCode;
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Visible { get; set; } = true;
    public bool Usable { get; set; } = true;
    public string UnusableReason { get; set; }
    public string[] ParamLabels { get; set; } = new string[] { };
    public object[] ParamDefaultValues { get; set; } = new object[] { };
    public object[][] ParamOptions { get; set; } = new object[][] { };
    public AboutTypeCodes TypeCode { get; }
}