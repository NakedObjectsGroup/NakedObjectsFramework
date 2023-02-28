using NakedFramework.RATL.Classic.Helpers;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Helpers;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestParameter : ITestParameter {
    private readonly Parameter parameter;

    public TestParameter(Parameter parameter, AcceptanceTestCase acceptanceTestCase) {
        this.parameter = parameter;
        AcceptanceTestCase = acceptanceTestCase;
    }

    internal AcceptanceTestCase AcceptanceTestCase { get; }
    public string Description => parameter.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.description].ToString();
    public bool IsOptional => (bool)parameter.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.optional];
    public bool IsMandatory => !IsOptional;
    public string Title => throw new NotImplementedException();
    public string Name => parameter.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.friendlyName].ToString();
    public Type Type => parameter.TypeToMatch();

    public ITestNaked[] GetChoices() {
        var valueChoices = parameter.GetChoices();
        if (valueChoices?.Any() == true) {
            return valueChoices.Select(v => new TestValue(v)).ToArray();
        }

        var choices = parameter.GetLinkChoices();
        return choices.Select(l => TestCaseHelpers.GetTestObject(l, AcceptanceTestCase)).Cast<ITestNaked>().ToArray();
    }

    public ITestNaked[] GetCompletions(string autoCompleteParm) => throw new NotImplementedException();

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
        Assert.IsTrue(Description == description, $"Parameter: {Name} description: {Description} expected: {description}");
        return this;
    }

    public ITestParameter AssertIsNamed(string name) {
        Assert.IsTrue(Name == name, $"Parameter name : {Name} expected {name}");
        return this;
    }

    public bool Match(Type type) => Type == typeof(Link) ? parameter.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType)?.ToString() == type.FullName : Type == type;
}