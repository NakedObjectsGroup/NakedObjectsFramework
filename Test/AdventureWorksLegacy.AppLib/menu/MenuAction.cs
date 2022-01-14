
namespace AdventureWorksLegacy.AppLib;

public class MenuAction : IMenuAction
{
    public MenuAction(string name) => Name = name.StartsWith("Action") ? name.Substring(6) : name;

    public string Name { get; init; }
}

