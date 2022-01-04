using System;

namespace NakedLegacy.Types;

public class Logical : ValueHolder<bool> {
    public Logical() { }

    public Logical(bool value) : base(value) { }

    public Logical(bool value, Action<bool> callback) : base(value, callback) { }
    public override object Parse(string fromString) => throw new NotImplementedException();
}