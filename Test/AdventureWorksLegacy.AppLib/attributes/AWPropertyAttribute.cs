using NakedFramework;

namespace AdventureWorksLegacy.AppLib;

[AttributeUsage(AttributeTargets.Property)]
public class AWPropertyAttribute : Attribute,
    IMemberOrderAttribute, IHiddenAttribute, INamedAttribute, IRequiredAttribute
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
}