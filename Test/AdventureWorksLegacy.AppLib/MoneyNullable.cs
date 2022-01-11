using System;
using System.Globalization;
using NakedLegacy.Types;

namespace AdventureWorksLegacy.AppLib;

public class MoneyNullable : ValueHolder<decimal?> {
    public MoneyNullable() { }

    public MoneyNullable(decimal? value) : base(value) { }

    public MoneyNullable(decimal? value, Action<decimal?> callback) : base(value, callback) { }

    public override string ToString() => ToString(Money.CURRENCY);

    private string ToString(string mask) => Value == null ? "" : Value.Value.ToString(mask, CultureInfo.InvariantCulture);

    public override object Parse(string entry) {
        try {
            return new MoneyNullable(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new ValueHolderException(entry);
        }
        catch (OverflowException) {
            throw new ValueHolderException(entry);
        }
    }

    public override object Display(string mask) => ToString(mask == null ? Money.CURRENCY : mask);

    public override ITitle Title() => new Title(ToString());
}