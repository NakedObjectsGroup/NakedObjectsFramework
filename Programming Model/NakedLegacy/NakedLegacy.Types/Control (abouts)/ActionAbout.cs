namespace NakedLegacy;

public interface ActionAbout : IAbout {
    string[] ParamLabels { get; set; }
    object[] ParamDefaultValues { get; set; }
    object[][] ParamOptions { get; set; }
}