using System;
using System.Globalization;


namespace Template.AppLib;

public class WholeNumberNullable : AbstractValueHolder<int?> {
    public WholeNumberNullable() { }

    // necessary for when used as a parameter
    public WholeNumberNullable(int? number) : base(number) { }

    public WholeNumberNullable(int? number, Action<int?> callback) : base(number, callback) { }

    public override string ToString() => Value == null ? "" :Value.Value.ToString();

    public override object Parse(string fromString) {
        try {
            return new WholeNumberNullable(int.Parse(fromString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            //throw new Exception(FormatMessage(fromString));
            throw new Exception(fromString);
        }
        catch (OverflowException) {
            //throw new Exception(OutOfRangeMessage(fromString, new WholeNumber(int.MinValue), new WholeNumber(int.MaxValue)));
            throw new Exception(fromString);
        }
    }
    
}