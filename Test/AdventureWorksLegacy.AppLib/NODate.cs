using System;
using System.Globalization;
using NakedLegacy;


namespace AdventureWorksLegacy.AppLib;
//This is needed to tell framework not to render the time portion
public class NODate : ValueHolder<DateTime>, IDateOnly{
    public NODate() { }

    public NODate(DateTime value) : base(value) { }

    public NODate(DateTime value, Action<DateTime> callback) : base(value, callback) { }

    public const string DATE_FORMAT = "dd/MM/yyyy";

    public override string ToString() => Value.ToString(Mask, Culture);

    public override string Mask => DATE_FORMAT;

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new ValueHolderException(dateString);
        }
    }

    public override object Display(string mask) => Value.ToString(mask, CultureInfo.InvariantCulture);

    public override Title Title() => new Title(ToString());
}