using System;
using System.Globalization;
using NakedLegacy;

namespace NakedLegacy.Rest.Test.Data.AppLib;

[DateOnly] //This is needed to tell framework not to render the time portion
public class NODateNullable : ValueHolder<DateTime?> {
    public NODateNullable() { }

    public NODateNullable(DateTime? value) : base(value) { }

    public NODateNullable(DateTime? value, Action<DateTime?> callback) : base(value, callback) { }

    private const string DATE_FORMAT = "dd/MM/yyyy";

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