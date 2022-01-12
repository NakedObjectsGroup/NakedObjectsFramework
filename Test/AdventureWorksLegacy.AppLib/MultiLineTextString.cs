using System;


namespace AdventureWorksLegacy.AppLib;

public class MultiLineTextString : TextString {
    public MultiLineTextString() { }

    public MultiLineTextString(string text) : base(text) { }

    public MultiLineTextString(string text, Action<string> callback) : base(text, callback) { }

    public override ITitle Title() => new Title(ToString());
}