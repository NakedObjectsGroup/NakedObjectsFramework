namespace NakedLegacy.Types {
    public interface ActionAbout : IAbout {
        string Name { get; set; }
        string Description { get; set; }
        string UnusableReason { get; set; }
        bool[] ParamsRequired { get; set; }
        string[] ParamLabels { get; set; }
        object[] ParamDefaultValues { get; set; }
        object[][] ParamOptions { get; set; }
        
    }
}