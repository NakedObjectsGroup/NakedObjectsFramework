
using System;

namespace NakedLegacy.Types {
    public class TextString : TitledObject {
        private string text;

        // necessary for when used as a parameter
        public TextString(string text) => Text = text;

        public TextString(string text, Action<string> callback) : this(text) => UpdateBackingField = callback;

        public string Text {
            get => text;
            set {
                text = value;
                UpdateBackingField(text);
            }
        }

        private Action<string> UpdateBackingField { get; } = _ => { };

        public override string ToString() => Text;

        public Title Title() => new Title(this);

    }
}