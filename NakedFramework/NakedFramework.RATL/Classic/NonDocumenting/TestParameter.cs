using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestParameter : ITestParameter {
    private readonly Parameter parameter;

    public TestParameter(Parameter parameter, AcceptanceTestCase acceptanceTestCase) {
        this.parameter = parameter;
        AcceptanceTestCase = acceptanceTestCase;
    }

    internal AcceptanceTestCase AcceptanceTestCase { get; }
    public string Title { get; }
    public string Name { get; }
    public ITestNaked[] GetChoices() => throw new NotImplementedException();

    public ITestNaked[] GetCompletions(string autoCompleteParm) => throw new NotImplementedException();

    public ITestNaked GetDefault() => throw new NotImplementedException();

    public ITestParameter AssertIsOptional() => throw new NotImplementedException();

    public ITestParameter AssertIsMandatory() => throw new NotImplementedException();

    public ITestParameter AssertIsDescribedAs(string description) => throw new NotImplementedException();

    public ITestParameter AssertIsNamed(string name) => throw new NotImplementedException();
}