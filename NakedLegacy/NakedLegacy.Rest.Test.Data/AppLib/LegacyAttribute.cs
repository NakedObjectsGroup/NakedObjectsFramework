using System;
using NakedFramework;
using NOF2.Attribute;

namespace NOF2.Rest.Test.Data.AppLib;

public class LegacyAttribute : System.Attribute, IMemberOrderAttribute, IMaxLengthAttribute, IRequiredAttribute,  INamedAttribute, IHiddenAttribute, ITableViewAttribute, IRestExtensionAttribute {
    public int MaxLength { get; set; }
    public int Order { get; set; }
    public string Name { get; set; }
    public WhenTo WhenTo { get; set; } = WhenTo.Never;
    public bool TableTitle { get; set; }
    public string[] TableColumns { get; set; }
    public string ExtensionName { get; set; }
    public string ExtensionValue { get; set;  }
    public bool IsRequired { get; set; } = false;
}