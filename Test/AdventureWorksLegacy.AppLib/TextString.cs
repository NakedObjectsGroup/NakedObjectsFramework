using System;


namespace AdventureWorksLegacy.AppLib;

public class TextString : ValueHolder<string> {
    public TextString() { }

    // necessary for when used as a parameter
    public TextString(string text) : base(text) { }

    public TextString(string text, Action<string> callback) : base(text, callback) { }

    public override string ToString() => Value;
    public override object Parse(string fromString) => new TextString(fromString);
    public override object Display(string mask = null) => Value;

    public override ITitle Title() => new Title(ToString());
}