namespace AdventureWorksLegacy.AppLib;

[AttributeUsage(AttributeTargets.Property)]
public class MemberOrderAttribute : Attribute, IMemberOrderAttribute 
{
    public MemberOrderAttribute(int order) => Order = order;

    public int Order { get; set; }
}