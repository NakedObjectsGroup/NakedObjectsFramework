using System;
using System.Globalization;


namespace AdventureWorksLegacy.AppLib;

public class WholeNumberNullable : ValueHolder<int?> {
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
            //throw new ValueHolderException(FormatMessage(fromString));
            throw new ValueHolderException(fromString);
        }
        catch (OverflowException) {
            //throw new ValueHolderException(OutOfRangeMessage(fromString, new WholeNumber(int.MinValue), new WholeNumber(int.MaxValue)));
            throw new ValueHolderException(fromString);
        }
    }

    public override object Display(string mask = null) => Value;

    public override ITitle Title() => new Title(ToString());
}