using System;

// ReSharper disable InconsistentNaming

namespace Legacy.Types {
    public class TextString {
        private readonly string text;

        public TextString(string text) => this.text = text;

        public TextString(string text, Action<string> callback) : this(text) => BackingField = callback;

        private Action<string> BackingField { get; set; } = _ => { };

        public override string ToString() => text;
    }
}