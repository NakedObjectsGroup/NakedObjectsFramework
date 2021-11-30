using System;
using System.Globalization;

namespace NakedLegacy.Types;

public class NODate : ValueHolder<DateTime>
{
    public NODate(DateTime value) : base(value) { }

    public NODate(DateTime value, Action<DateTime> callback) : base(value, callback) { }

    public override string ToString() => Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
