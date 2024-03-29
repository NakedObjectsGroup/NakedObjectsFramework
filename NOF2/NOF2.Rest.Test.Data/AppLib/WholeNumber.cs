﻿using System;
using System.Globalization;
using NOF2.Title;

namespace NOF2.Rest.Test.Data.AppLib;

public class WholeNumber : ValueHolder<int> {
    public WholeNumber() { }

    // necessary for when used as a parameter
    public WholeNumber(int number) : base(number) { }

    public WholeNumber(int number, Action<int> callback) : base(number, callback) { }

    public override string ToString() => Value.ToString();

    public override object Parse(string fromString) {
        try {
            return new WholeNumber(int.Parse(fromString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            //throw new ValueHolderException(FormatMessage(fromString));
            throw new Exception(fromString);
        }
        catch (OverflowException) {
            //throw new ValueHolderException(OutOfRangeMessage(fromString, new WholeNumber(int.MinValue), new WholeNumber(int.MaxValue)));
            throw new Exception(fromString);
        }
    }

    public override object Display(string mask = null) => Value;

    public override ITitle Title() => new Title(ToString());
}