using System;
using System.Globalization;

namespace NakedLegacy.Types;

public class NODate : ValueHolder<DateTime> {
    public NODate() { }

    public NODate(DateTime value) : base(value) { }

    public NODate(DateTime value, Action<DateTime> callback) : base(value, callback) { }

    public override string ToString() => Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new NODate(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new ValueHolderException(dateString);
        }
    }

    public override object Display(string mask = null) => Value.ToString(mask, CultureInfo.InvariantCulture);
}