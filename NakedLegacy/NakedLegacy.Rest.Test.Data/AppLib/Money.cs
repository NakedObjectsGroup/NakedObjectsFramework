using System;
using System.Globalization;

namespace NakedLegacy.Rest.Test.Data.AppLib; 

public class Money : ValueHolder<decimal> {
    public Money() { }

    public Money(decimal value) : base(value) { }

    public Money(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override string Mask => "C";

    public override string ToString() => "€ " + Value;

    public override object Parse(string entry) {
        try {
            return new Money(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new ValueHolderException(entry);
        }
        catch (OverflowException) {
            throw new ValueHolderException(entry);
        }
    }

    public override object Display(string mask = null) => Value;

    public override ITitle Title() => new Title(ToString());
}