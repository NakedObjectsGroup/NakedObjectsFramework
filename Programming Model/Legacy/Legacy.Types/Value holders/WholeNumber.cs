
using System;

namespace NakedLegacy.Types {
    public class WholeNumber : TitledObject {
        private int number;

        // necessary for when used as a parameter
        public WholeNumber(int number) => Number = number;

        public WholeNumber(int number, Action<int> callback) : this(number) => UpdateBackingField = callback;

        public int Number {
            get => number;
            set {
                number = value;
                UpdateBackingField(number);
            }
        }

        private Action<int> UpdateBackingField { get; } = _ => { };

        public override string ToString() => number.ToString();

        public Title Title() => new Title(this);
    }
}