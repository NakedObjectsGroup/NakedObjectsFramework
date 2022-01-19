using System;
using System.Globalization;


namespace AdventureWorksLegacy.AppLib;

public class Percentage : ValueHolder<decimal> {
    public Percentage() { }

    public Percentage(decimal value) : base(value) { }

    public Percentage(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override string ToString() => $"{Value} %";

    public override object Parse(string entry) {
        try {
            return new Percentage(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new ValueHolderException(entry);
        }
        catch (OverflowException) {
            throw new ValueHolderException(entry);
        }
    }

    public override object Display(string mask = null) => Value;

    public override Title Title() => new Title(ToString());
}