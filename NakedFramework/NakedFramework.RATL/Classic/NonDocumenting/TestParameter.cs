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

    public TestParameter(Parameter parameter, AcceptanceTestCase acceptanceTestCase) {
        this.parameter = parameter;
        AcceptanceTestCase = acceptanceTestCase;
    }

    internal AcceptanceTestCase AcceptanceTestCase { get; }
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

    public ITestNaked[] GetChoices() => GetChoices(parameter);

    public ITestNaked[] GetCompletions(string autoCompleteParm) {
        var prompt = parameter.GetPrompts(AcceptanceTestCase.TestInvokeOptions(), autoCompleteParm).Result;
        return GetChoices(prompt);
    }

    public ITestNaked GetDefault() {
        if (parameter.GetLinkDefault() is { } link) {
            var domainObject = ROSIApi.GetObject(link.GetHref(), AcceptanceTestCase.TestInvokeOptions()).Result;
            return new TestObject(domainObject, AcceptanceTestCase);
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

    private ITestNaked[] GetChoices(IHasChoices hasChoices) {
        var valueChoices = hasChoices.GetChoices().ToArray();
        if (valueChoices?.Any() == true) {
            return valueChoices.Select(v => new TestValue(v)).Cast<ITestNaked>().ToArray();
        }

        return hasChoices.GetLinkChoices().Select(l => RATLHelpers.GetTestObject(l, AcceptanceTestCase)).Cast<ITestNaked>().ToArray();
    }
}