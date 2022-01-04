using System;

namespace NakedLegacy.Types;

public class MultiLineTextString : TextString {
    public MultiLineTextString() { }

    public MultiLineTextString(string text) : base(text) { }

    public MultiLineTextString(string text, Action<string> callback) : base(text, callback) { }
}