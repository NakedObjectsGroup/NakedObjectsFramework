
namespace AdventureWorksLegacy.AppLib;

public class MenuAction : IMenuAction
{
    public MenuAction(string name) => Name = name;

    public string Name { get; init; }
}

