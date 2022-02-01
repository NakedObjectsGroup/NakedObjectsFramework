using System;
using System.Globalization;


namespace Template.AppLib;

public class WholeNumber : AbstractValueHolder<int> {
    public WholeNumber() {}

    // necessary for when used as a parameter
    public WholeNumber(int number) : base(number) { }

    public WholeNumber(int number, Action<int> callback) : base(number, callback) { }

    //Used only for initialisation on a transient object
    public WholeNumber(Action<int> callback) : base(callback) { }

    public override string ToString() => Value.ToString();

    public override object Parse(string fromString) {
        try {
            return new WholeNumber(int.Parse(fromString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
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