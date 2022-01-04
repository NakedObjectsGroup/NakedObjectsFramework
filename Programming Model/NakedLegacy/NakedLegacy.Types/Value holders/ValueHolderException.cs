using System;

namespace NakedLegacy.Types;

public class ValueHolderException : Exception {
    public ValueHolderException(string msg) : base(msg) { }
}