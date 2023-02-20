using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting; 

internal class TestObject : ITestObject {
    internal AcceptanceTestCase AcceptanceTestCase { get; }
    private readonly DomainObject domainObject;

    public TestObject(DomainObject domainObject, AcceptanceTestCase acceptanceTestCase) {
        AcceptanceTestCase = acceptanceTestCase;
        this.domainObject = domainObject;
    }

    public string Title => domainObject.GetTitle();
    public ITestAction[] Actions => domainObject.GetActions().Select(a => new TestAction(a, AcceptanceTestCase)).Cast<ITestAction>().ToArray();
    public ITestAction GetAction(string name) => Actions.SingleOrDefault(a => a.Name == name);

    public ITestAction GetAction(string name, params Type[] parameterTypes) => throw new NotImplementedException();

    public ITestAction GetAction(string name, string subMenu) => throw new NotImplementedException();

    public ITestAction GetAction(string name, string subMenu, params Type[] parameterTypes) => throw new NotImplementedException();

    public string GetObjectActionOrder() => throw new NotImplementedException();

    public ITestHasActions AssertActionOrderIs(string order) => throw new NotImplementedException();

    public ITestProperty[] Properties { get; }
    public ITestObject Save() => throw new NotImplementedException();

    public ITestObject Refresh() => throw new NotImplementedException();

    public ITestProperty GetPropertyByName(string name) => throw new NotImplementedException();

    public ITestProperty GetPropertyById(string id) => throw new NotImplementedException();

    public ITestObject AssertCanBeSaved() => throw new NotImplementedException();

    public ITestObject AssertCannotBeSaved() => throw new NotImplementedException();

    public ITestObject AssertIsTransient() => throw new NotImplementedException();

    public ITestObject AssertIsPersistent() => throw new NotImplementedException();

    public string GetPropertyOrder() => throw new NotImplementedException();

    public ITestHasProperties AssertPropertyOrderIs(string order) => throw new NotImplementedException();

    public ITestObject AssertIsImmutable() => throw new NotImplementedException();

    public ITestObject AssertIsDescribedAs(string expectedDescription) => throw new NotImplementedException();

    public ITestObject AssertIsType(Type expectedType) => throw new NotImplementedException();

    public ITestObject AssertTitleEquals(string expectedTitle) {
        var actualTitle = Title;
        Assert.IsTrue(expectedTitle == actualTitle, $"Expected title '{expectedTitle}' but got '{actualTitle}'");
        return this;
    }
}