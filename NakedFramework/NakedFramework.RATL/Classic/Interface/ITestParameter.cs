namespace NakedFramework.RATL.Classic.Interface;

public interface ITestParameter : ITestNaked {
    string Name { get; }
    Type Type { get; }
    ITestNaked[] GetChoices();
    ITestNaked[] GetCompletions(string autoCompleteParm);
    ITestNaked GetDefault();
    ITestParameter AssertIsOptional();
    ITestParameter AssertIsMandatory();
    ITestParameter AssertIsDescribedAs(string description);
    ITestParameter AssertIsNamed(string name);
    bool Match(Type type);
}