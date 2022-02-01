using System;
using System.Globalization;

namespace Template.AppLib;

public class FloatingPointNumber : AbstractValueHolder<decimal> {
    public FloatingPointNumber() { }

    public FloatingPointNumber(decimal value) : base(value) { }

    public FloatingPointNumber(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override string ToString() => Value.ToString();

    public override object Parse(string entry) {
        try {
            return new FloatingPointNumber(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new Exception(entry);
        }
        catch (OverflowException) {
            throw new Exception(entry);
        }
    }
    
}