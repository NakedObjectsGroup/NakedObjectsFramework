
using System;

namespace NakedLegacy.Types {
    public class Logical : TitledObject {
        private bool b;

        // necessary for when used as a parameter
        public Logical(bool value) => b = value;

        public Logical(bool value, Action<bool> callback) : this(value) => UpdateBackingField = callback;

        public bool Boolean {
            get => b;
            set {
                b = value;
                UpdateBackingField(b);
            }
        }

        private Action<bool> UpdateBackingField { get; } = _ => { };

        public override string ToString() => b.ToString();

        public Title Title() => new Title(this);

    }
}