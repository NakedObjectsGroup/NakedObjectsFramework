using System;

namespace NakedLegacy.Types;

public class TimeStamp : ValueHolder<DateTime> {
    public TimeStamp() { }

    // necessary for when used as a parameter
    public TimeStamp(DateTime dateTime) : base(dateTime) { }

    public TimeStamp(DateTime dateTime, Action<DateTime> callback) : base(dateTime, callback) { }

    public override string ToString() => Value.ToString("dd/MM/yyyy hh:mm:ss"); //TODO: match original format.
    public override object Parse(string fromString) => throw new NotImplementedException();
    public override object Display(string mask = null) => Value;
}