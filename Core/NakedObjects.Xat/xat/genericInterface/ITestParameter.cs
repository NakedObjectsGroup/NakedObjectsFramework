namespace NakedObjects.Xat.Generic {
    public interface ITestParameter : ITestNaked {
        string Name { get; }
        ITestNaked[] GetChoices();
        ITestNaked GetDefault();
        ITestParameter AssertIsOptional();
        ITestParameter AssertIsMandatory();
        ITestParameter AssertIsDescribedAs(string description);
        ITestParameter AssertIsNamed(string name);
    }
}