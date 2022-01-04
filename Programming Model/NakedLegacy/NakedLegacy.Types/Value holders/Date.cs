using System;
using System.Globalization;

namespace NakedLegacy.Types;

public class Date : ValueHolder<DateTime> {
    public Date() { }

    // necessary for when used as a parameter
    public Date(DateTime dateTime) : base(dateTime) { }

    public Date(DateTime dateTime, Action<DateTime> callback) : base(dateTime, callback) { }

    public override string ToString() => Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); //TODO: match original format.
    public override object Parse(string fromString) => throw new NotImplementedException();
    public override object Display(string mask = null) => Value;
}