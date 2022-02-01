using System;
using System.Globalization;


namespace Template.AppLib;

public class Money : AbstractValueHolder<decimal> {
    public Money() { }

    public Money(decimal value) : base(value) { }

    public Money(decimal value, Action<decimal> callback) : base(value, callback) { }

    internal const string CURRENCY = "C";

    public override string ToString() => Value.ToString(CURRENCY, Culture);

    public override string Mask => CURRENCY;

    public override object Parse(string entry) {
        try {
            return new Money(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new Exception(entry);
        }
        catch (OverflowException) {
            throw new Exception(entry);
        }
    }

    
}