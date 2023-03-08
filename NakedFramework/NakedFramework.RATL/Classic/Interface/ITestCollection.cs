namespace NakedFramework.RATL.Classic.Interface;

public interface ITestCollection : ITestNaked, IEnumerable<ITestObject> {
    ITestCollection AssertIsEmpty();
    ITestCollection AssertIsNotEmpty();
    ITestCollection AssertCountIs(int count);
    ITestCollection AssertIsTransient();
    ITestCollection AssertIsPersistent();
}