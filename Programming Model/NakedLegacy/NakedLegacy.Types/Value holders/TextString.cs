using System;

namespace NakedLegacy.Types;

public class TextString : ValueHolder<string> {
    public TextString() { }

    // necessary for when used as a parameter
    public TextString(string text) : base(text) { }

    public TextString(string text, Action<string> callback) : base(text, callback) { }

    public string Text => Value;

    public override string ToString() => Text;
    public override string Parse(string fromString) => throw new NotImplementedException();
}