using System;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public class LegacyAttribute : Attribute, IMemberOrderAttribute, IMaxLengthAttribute {
    public int MaxLength { get; set; }
    public int Order { get; set; }
}