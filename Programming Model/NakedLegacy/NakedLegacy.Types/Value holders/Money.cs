
using System;

namespace NakedLegacy.Types {
    public class Money : TitledObject {
        private decimal d;

        // necessary for when used as a parameter
        public Money(decimal value) => Value = value;

        public Money(decimal value, Action<decimal> callback) : this(value) => UpdateBackingField = callback;

        public decimal Value {
            get => d;
            set {
                d = value;
                UpdateBackingField(d);
            }
        }

        private Action<decimal> UpdateBackingField { get; } = _ => { };

        public override string ToString() => d.ToString();  //TODO: Check format

        public Title Title() => new Title(this);

    }
}