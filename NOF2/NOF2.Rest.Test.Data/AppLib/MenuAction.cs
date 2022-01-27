using NOF2.Menu;

namespace NOF2.Rest.Test.Data.AppLib;

public class MenuAction : IMenuAction {
    public MenuAction(string name) => Name = name;
    public string Name { get; set; }
    public string DisplayName { get; set; }
}