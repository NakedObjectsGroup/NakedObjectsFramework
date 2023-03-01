using ROSI.Records;

namespace NakedFramework.RATL.Classic.Interface;

public interface ITestObject : ITestNaked, ITestHasActions, ITestHasProperties
{
    ITestObject AssertIsImmutable();
    ITestObject AssertIsDescribedAs(string expectedDescription);
    ITestObject AssertIsType(Type expectedType);
    ITestObject AssertTitleEquals(string expectedTitle);
    DomainObject Value { get; }
}