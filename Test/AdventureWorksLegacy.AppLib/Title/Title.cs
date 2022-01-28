

namespace AdventureWorks.NOF2.AppLib;

public class Title : ITitle {
    private readonly string text = "";

    public Title() { }

    public Title(object obj) : this() => text += obj;

    public Title(ITitledObject obj) : this() => text += obj.Title().TitleString();

    public Title(ITitledObject obj, string defaultValue) : this() =>
        text += TitleString().Length is 0 ? defaultValue : obj.Title().ToString();

    public override string ToString() => text;

    public string TitleString() => text;
}