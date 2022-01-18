namespace NakedLegacy.Rest.Test.Data.AppLib; 

public class MenuAction : IMenuAction {
    public MenuAction(string name) => Name = name;
    public string Name { get; set; }
    public string DisplayName { get; set; }
}