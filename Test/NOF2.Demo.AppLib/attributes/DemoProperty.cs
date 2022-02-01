using NakedFramework;

namespace NOF2.Demo.AppLib;

[AttributeUsage(AttributeTargets.Property)]
public class DemoPropertyAttribute : System.Attribute,
    IMemberOrderAttribute, IHiddenAttribute, INamedAttribute, IRequiredAttribute
{
    public DemoPropertyAttribute() { }

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
}