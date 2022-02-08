using NOF2.Attribute;
using NOF2.Enum;

namespace NOF2.Rest.Test.Data.AppLib;

public class NOF2Attribute : System.Attribute, IMemberOrderAttribute, IMaxLengthAttribute, IRequiredAttribute, INamedAttribute, IHiddenAttribute, ITableViewAttribute, IRestExtensionAttribute {
    public WhenTo WhenTo { get; set; } = WhenTo.Never;
    public int MaxLength { get; set; }
    public int Order { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; } = false;
    public string ExtensionName { get; set; }
    public string ExtensionValue { get; set; }
    public bool TableTitle { get; set; }
    public string[] TableColumns { get; set; }
}