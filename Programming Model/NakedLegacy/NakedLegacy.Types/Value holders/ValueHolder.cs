
using System;

namespace NakedLegacy.Types {
    public abstract class ValueHolder<T> : TitledObject {
        private T value;

        public ValueHolder(T value) => this.value = value;

        public ValueHolder(T value, Action<T> callback) : this(value) => UpdateBackingField = callback;

        public T Value {
            get => value;
            set {
                this.value = value;
                UpdateBackingField(value);
            }
        }

        private Action<T> UpdateBackingField { get; } = _ => { };

        public override string ToString() => value.ToString();

        public Title Title() => new Title(Value);

    }
}