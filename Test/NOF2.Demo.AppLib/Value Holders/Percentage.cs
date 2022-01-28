using System;
using System.Globalization;


namespace NOF2.Demo.AppLib;

public class Percentage : AbstractValueHolder<decimal> {
    public Percentage() { }

    public Percentage(decimal value) : base(value) { }

    public Percentage(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override object Parse(string entry) {
        try {
            return new Percentage(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new Exception(entry);
        }
        catch (OverflowException) {
            throw new Exception(entry);
        }
    }

    
}