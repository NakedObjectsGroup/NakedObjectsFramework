using System;

namespace Legacy.NakedObjects.Application.ValueHolder
{
    public class WholeNumber
    {
        public int value { get; set; }
        public Action<int> update { get; set; } = _ => { };

        public WholeNumber(int value, Action<int> udpateMappedField) : this(value) => update = udpateMappedField;

        public WholeNumber(int value) => this.value = value;
    }
}
