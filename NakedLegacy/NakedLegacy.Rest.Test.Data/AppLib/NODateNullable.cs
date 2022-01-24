using System;
using System.Globalization;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public class NODateNullable : ValueHolder<DateTime?>, IDateOnly {
    //This is needed to tell framework not to render the time portion

    private const string DATE_FORMAT = "dd/MM/yyyy";
    public NODateNullable() { }

    public NODateNullable(DateTime? value) : base(value) { }

    public NODateNullable(DateTime? value, Action<DateTime?> callback) : base(value, callback) { }

    public override string ToString() => Value == null ? "" : Value.Value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new ValueHolderException(dateString);
        }
    }

    public override object Display(string mask = DATE_FORMAT) => Value == null ? "" : Value.Value.ToString(mask == null ? DATE_FORMAT : mask, CultureInfo.InvariantCulture);

    public override ITitle Title() => new Title(ToString());
}