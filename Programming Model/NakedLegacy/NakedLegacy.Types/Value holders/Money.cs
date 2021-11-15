
using System;

namespace NakedLegacy.Types {

    public class Money : ValueHolder<decimal>
    {
        public Money(decimal value) : base(value) { }

        public Money(decimal value, Action<decimal> callback) : base(value, callback) { }

        public override string ToString() => "€ " + Value;
    }
}