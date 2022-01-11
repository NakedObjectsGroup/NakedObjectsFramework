using System;
using System.Globalization;
using NakedLegacy.Types;

namespace AdventureWorksLegacy.AppLib;

public class Money : ValueHolder<decimal> {
    public Money() { }

    public Money(decimal value) : base(value) { }

    public Money(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override string ToString() => ToString(CURRENCY);

    internal string ToString(string mask) => Value.ToString(mask, CultureInfo.InvariantCulture);


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
    internal const string CURRENCY = "€0.00";

    public override object Display(string mask = CURRENCY) => ToString(mask);

    public override ITitle Title() => new Title(ToString());
}