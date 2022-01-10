using System;
using System.Globalization;
using NakedLegacy;
using NakedLegacy.Types;

namespace AdventureWorksLegacy.AppLib;

[DateOnly] //This is needed to tell framework not to render the time portion
public class NODateNullable : ValueHolder<DateTime?> {
    public NODateNullable() { }

    public NODateNullable(DateTime? value) : base(value) { }

    public NODateNullable(DateTime? value, Action<DateTime?> callback) : base(value, callback) { }

    public override string ToString() => Value == null ? "" : Value.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new ValueHolderException(dateString);
        }
    }

    public override object Display(string mask = null) => Value == null ? "" : Value.Value.ToString(mask, CultureInfo.InvariantCulture);

    public override ITitle Title() => new Title(ToString());
}