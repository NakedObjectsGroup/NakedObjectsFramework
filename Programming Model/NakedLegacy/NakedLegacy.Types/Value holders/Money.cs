using System;

namespace NakedLegacy.Types;

public class Money : ValueHolder<decimal> {
    public Money() { }

    public Money(decimal value) : base(value) { }

    public Money(decimal value, Action<decimal> callback) : base(value, callback) { }

    public override string ToString() => "€ " + Value;
    public override object Parse(string fromString) => throw new NotImplementedException();
    public override object Display(string mask = null) => Value;
}