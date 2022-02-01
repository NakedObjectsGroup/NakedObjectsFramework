using System.Globalization;


namespace Template.AppLib;

public class NODateNullable : AbstractValueHolder<DateTime?>, IDateOnly
{
    public NODateNullable() { }

    public NODateNullable(DateTime? value) : base(value) { }

    public NODateNullable(DateTime? value, Action<DateTime?> callback) : base(value, callback) { }

    public override string ToString() => Value == null ? "" : Value.Value.ToString(Mask, Culture);

    public override string Mask => NODate.DATE_FORMAT;

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new Exception(dateString);
        }
    }

    public override object Display(string mask = null) => Value == null? "": Value.Value.ToString(mask, CultureInfo.InvariantCulture);

    
}