﻿using System;
using System.Globalization;
using NakedLegacy.Types;

namespace AdventureWorksLegacy.AppLib;

public class FloatingPointNumberNullable : ValueHolder<decimal?> {
    public FloatingPointNumberNullable() { }

    public FloatingPointNumberNullable(decimal? value) : base(value) { }

    public FloatingPointNumberNullable(decimal? value, Action<decimal?> callback) : base(value, callback) { }

    public override string ToString() => Value == null? "": Value.Value.ToString();

    public override object Parse(string entry) {
        try {
            return new FloatingPointNumber(decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new ValueHolderException(entry);
        }
        catch (OverflowException) {
            throw new ValueHolderException(entry);
        }
    }

    public override object Display(string mask = null) => Value;

    public override ITitle Title() => new Title(ToString());
}