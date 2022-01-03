using System;
using System.Globalization;

namespace NakedLegacy.Types; 

public class Date : ValueHolder<DateTime> {
    // necessary for when used as a parameter
    public Date(DateTime dateTime) : base(dateTime) { }

    public Date(DateTime dateTime, Action<DateTime> callback) : base(dateTime, callback) { }

    public DateTime DateTime => Value;

    public override string ToString() => DateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); //TODO: match original format.
}