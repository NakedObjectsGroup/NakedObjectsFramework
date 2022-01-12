namespace NakedLegacy;

public interface ActionAbout : IAbout {
    bool[] ParamsRequired { get; set; }
    string[] ParamLabels { get; set; }
    object[] ParamDefaultValues { get; set; }
    object[][] ParamOptions { get; set; }
}