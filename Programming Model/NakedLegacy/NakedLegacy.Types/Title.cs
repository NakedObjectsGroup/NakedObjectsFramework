using System.Text;

namespace NakedLegacy.Types; 

public class Title {
    private readonly StringBuilder stringBuilder;

    public Title() => stringBuilder = new StringBuilder();

    public Title(object text) : this() => stringBuilder.Append(text);

    public Title(ITitledObject obj) : this() => stringBuilder.Append(TitleString(obj));

    public Title(ITitledObject obj, string defaultValue) : this() {
        if (TitleString(obj).Length is 0) {
            stringBuilder.Append(defaultValue);
        }
        else {
            stringBuilder.Append(obj.Title());
        }
    }

    public override string ToString() => stringBuilder.ToString();

    public static string TitleString(ITitledObject obj) => obj?.ToString() ?? "";
}