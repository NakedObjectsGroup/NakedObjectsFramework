namespace Template.AppLib;

[AttributeUsage(AttributeTargets.Property)]
public class MemberOrderAttribute : System.Attribute, IMemberOrderAttribute 
{
    public MemberOrderAttribute(int order) => Order = order;

    public int Order { get; set; }
}