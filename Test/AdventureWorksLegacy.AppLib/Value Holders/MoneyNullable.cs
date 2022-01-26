using System;
using System.Globalization;


namespace AdventureWorksLegacy.AppLib;

public class MoneyNullable : AbstractValueHolder<decimal?> {
    public MoneyNullable() { }

    public MoneyNullable(decimal? value) : base(value) { }

    public MoneyNullable(decimal? value, Action<decimal?> callback) : base(value, callback) { }

    public override string ToString() => Value == null ? "" : Value.Value.ToString(Mask, Culture);

    public override string Mask => Money.CURRENCY;

    public override object Parse(string entry) {
        try {
            return new MoneyNullable(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new Exception(entry);
        }
        catch (OverflowException) {
            throw new Exception(entry);
        }
    }

    
}