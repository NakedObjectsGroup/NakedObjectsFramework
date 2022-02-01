using System.Globalization;


namespace Template.AppLib;
//This is needed to tell framework not to render the time portion
public class NODate : AbstractValueHolder<DateTime>, IDateOnly{
    public NODate() { }

    public NODate(DateTime value) : base(value) { 
     if (value == new DateTime()) IsNull = true;
    }

    public NODate(DateTime value, Action<DateTime> callback) : base(value, callback) {
        if (value == new DateTime()) IsNull = true;
    }

    public const string DATE_FORMAT = "dd/MM/yyyy";

    public override string ToString() => Value.ToString(Mask, Culture);

    public override string Mask => DATE_FORMAT;

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new Exception(dateString);
        }
    }

    public override object Display(string mask) => IsNull ? "" :Value.ToString(mask, CultureInfo.InvariantCulture);

    
}