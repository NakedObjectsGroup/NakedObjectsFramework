
namespace AdventureWorksLegacy.AppLib;

public class MenuAction : IMenuAction
{
    public MenuAction(string name) => Name = name.ToLower().StartsWith("action") ? name : "Action" + name;

    public string Name { get; init; }
}

