using System;

namespace NakedLegacy.Types;

public class WholeNumber : ValueHolder<int> {
    // necessary for when used as a parameter
    public WholeNumber(int number) : base(number) { }

    public WholeNumber(int number, Action<int> callback) : base(number, callback) { }

    public int Number => Value;

    public override string ToString() => Number.ToString();
}