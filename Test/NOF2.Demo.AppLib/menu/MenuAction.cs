
namespace NOF2.Demo.AppLib;

public class MenuAction : IMenuAction
{
    public MenuAction(string methodName, string displayName = null)
    {
        Name = methodName.ToLower().StartsWith("action") ? methodName : "Action" + methodName;
        DisplayName = displayName;
    }

    public string Name { get; init; }

    public string DisplayName { get; init; }
}

