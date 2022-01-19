using System;


namespace AdventureWorksLegacy.AppLib;

public class Logical : ValueHolder<bool> {
    public Logical() { }

    public Logical(bool value) : base(value) { }

    public Logical(bool value, Action<bool> callback) : base(value, callback) { }

    public override object Parse(string entry) {
        if ("true".StartsWith(entry.ToLower())) {
            return new Logical(true);
        }

        if ("false".StartsWith(entry.ToLower())) {
            return new Logical(false);
        }

        throw new ValueHolderException(entry);
    }

    public override object Display(string mask = null) => Value;

    public override Title Title() => new Title(ToString());
}