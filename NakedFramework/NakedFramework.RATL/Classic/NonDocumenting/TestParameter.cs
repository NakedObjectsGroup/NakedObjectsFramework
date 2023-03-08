using NakedFramework.RATL.Classic.Helpers;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;


namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestParameter : ITestParameter {
    private readonly Parameter parameter;

    public TestParameter(Parameter parameter) {
        this.parameter = parameter;
       
    }

  
    public string Description => parameter.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.description);
    public bool IsOptional => parameter.GetExtensions().GetExtension<bool>(ExtensionsApi.ExtensionKeys.optional);
    public bool IsMandatory => !IsOptional;
    public string Title => throw new NotImplementedException();
    public string Name => parameter.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.friendlyName);

    public Type Type =>
        parameter.TypeToMatch() switch {
            { } t when t == typeof(Link) => RATLHelpers.GetType(parameter.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType)),
            { } t => t,
            _ => null
        };

    public ITestNaked[] GetChoices() => RATLHelpers.GetChoices(parameter);

    public ITestNaked[] GetCompletions(string autoCompleteParm) {
        var prompt = parameter.GetPrompts(autoCompleteParm).Result;
        return RATLHelpers.GetChoices(prompt);
    }

    public ITestNaked GetDefault() {
        if (parameter.GetLinkDefault() is { } link) {
            var domainObject = ROSIApi.GetObject(link.GetHref(), link.Options).Result;
            return new TestObject(domainObject);
        }

        return parameter.HasDefault() ? new TestValue(parameter.GetDefault()) : null;
    }

    public ITestParameter AssertIsOptional() {
        Assert.IsTrue(IsOptional, $"Parameter: {Name} is mandatory");
        return this;
    }

    public ITestParameter AssertIsMandatory() {
        Assert.IsTrue(IsMandatory, $"Parameter: {Name} is optional");
        return this;
    }

    public ITestParameter AssertIsDescribedAs(string description) {
        Assert.IsTrue(Description == description, $"Parameter: {Name} description: '{Description}' expected: '{description}'");
        return this;
    }

    public ITestParameter AssertIsNamed(string name) {
        Assert.IsTrue(Name == name, $"Parameter name :'{Name}' expected '{name}'");
        return this;
    }

    public bool Match(Type type) => Type == type;

   
}