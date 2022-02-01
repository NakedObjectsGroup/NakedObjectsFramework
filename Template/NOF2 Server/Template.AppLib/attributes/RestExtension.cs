namespace Template.AppLib;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class RestExtensionAttribute : System.Attribute, IRestExtensionAttribute
{
    public RestExtensionAttribute(string name, string value)
    {
        ExtensionName = name;
        ExtensionValue = value;
    }
    public string ExtensionName { get; set; }
    public string ExtensionValue { get; set; }
}

