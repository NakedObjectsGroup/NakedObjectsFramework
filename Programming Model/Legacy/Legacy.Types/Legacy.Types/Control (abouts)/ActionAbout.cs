namespace Legacy.Types {
    public interface ActionAbout {
        string Name { get; set; }
        string Description { get; set; }
        bool Visible { get; set; }
        bool Usable { get; set; }
        string UnusableReason { get; set; }
        string[] ParamLabels { get; set; }
        object[] ParamDefaultValues { get; set; }
        object[][] ParamOptions { get; set; }
        AboutType Mode();
    }
}