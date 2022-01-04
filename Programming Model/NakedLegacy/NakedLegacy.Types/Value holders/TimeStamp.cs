using System;

namespace NakedLegacy.Types;

public class TimeStamp : ValueHolder<DateTime> {
    public TimeStamp() { }

    // necessary for when used as a parameter
    public TimeStamp(DateTime dateTime) : base(dateTime) { }

    public TimeStamp(DateTime dateTime, Action<DateTime> callback) : base(dateTime, callback) { }

    public DateTime DateTime => Value;

    public override string ToString() => DateTime.ToString("dd/MM/yyyy hh:mm:ss"); //TODO: match original format.
    public override object Parse(string fromString) => throw new NotImplementedException();
}