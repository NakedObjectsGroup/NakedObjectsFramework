using System;
using NakedLegacy.Attribute;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public class LegacyAttribute : System.Attribute, IMemberOrderAttribute, IMaxLengthAttribute {
    public int MaxLength { get; set; }
    public int Order { get; set; }
}