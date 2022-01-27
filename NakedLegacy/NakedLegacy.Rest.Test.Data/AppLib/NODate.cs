using System;
using System.Globalization;
using NOF2.Interface;
using NOF2.Title;
using NOF2.ValueHolder;

namespace NOF2.Rest.Test.Data.AppLib;

public class NODate : ValueHolder<DateTime>, IDateOnly {
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
            throw new Exception(dateString);
        }
    }

    public override object Display(string mask = null) => Value.ToString(mask, CultureInfo.InvariantCulture);

    public override ITitle Title() => new Title(ToString());
}