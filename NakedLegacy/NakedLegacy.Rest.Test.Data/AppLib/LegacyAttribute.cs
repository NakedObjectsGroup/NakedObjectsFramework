using System;
using NakedLegacy;

namespace NakedLegacy.Rest.Test.Data.AppLib; 


public class LegacyAttribute : Attribute, IMemberOrderAttribute, IMaxLengthAttribute {
    public int Order { get; set; }
    public int MaxLength { get; set; }
}