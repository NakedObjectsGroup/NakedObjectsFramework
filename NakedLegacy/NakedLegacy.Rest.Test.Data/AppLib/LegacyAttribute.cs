using System;

namespace NakedLegacy.Rest.Test.Data.AppLib; 

public class LegacyAttribute : Attribute, IMemberOrderAttribute {
    public int Order { get; set; }
}