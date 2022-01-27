using NakedFramework;

[AttributeUsage(AttributeTargets.Property)]
public class AWPropertyAttribute : Attribute,
    IMemberOrderAttribute, IHiddenAttribute, INamedAttribute, IRequiredAttribute, IRestExtensionAttribute
{
    public AWPropertyAttribute() { }

    public int Order { get; set; }
    public string Name { get; set; }

    public WhenTo WhenTo { get; set; } = WhenTo.Never;

    public bool Hidden
    {
        get
        {
            return WhenTo == WhenTo.Always;
        }
        set
        {
            if (value)
            { WhenTo = WhenTo.Always; }
            else { WhenTo = WhenTo.Never; }
        }
    }

    public bool IsRequired { get; set; }
    public string ExtensionName { get; set; }
    public string ExtensionValue { get; set; }
}