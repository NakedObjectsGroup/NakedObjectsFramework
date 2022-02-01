using System;


namespace Template.AppLib;

public class TimeStamp : AbstractValueHolder<DateTime> {
    public TimeStamp() { }

    // necessary for when used as a parameter
    public TimeStamp(DateTime dateTime) : base(dateTime) { }

    public TimeStamp(DateTime dateTime, Action<DateTime> callback) : base(dateTime, callback) { }

    public override string ToString() => Value.ToString("dd/MM/yyyy hh:mm:ss"); //TODO: match original format.

    public override object Parse(string entry) {
        var dateString = entry.Trim();
        try {
            return new TimeStamp(DateTime.Parse(entry));
        }
        catch (FormatException) {
            throw new Exception(dateString);
        }
    }

    
}