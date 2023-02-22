using System.Text;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestObject : TestHasActions, ITestObject {
    public TestObject(DomainObject domainObject, AcceptanceTestCase acceptanceTestCase) : base(domainObject, acceptanceTestCase) { }
    private bool IsTransient => IsInteractionMode("transient");
    private bool IsPersistent => IsInteractionMode("persistent");

    public override string Title {
        get {
            Assert.IsNotNull(DomainObject, "Cannot get title for null object");
            return DomainObject?.GetTitle();
        }
    }

    public ITestProperty[] Properties => DomainObject.GetProperties().Select(p => new TestProperty(p, AcceptanceTestCase)).Cast<ITestProperty>().ToArray();

    public ITestObject AssertTitleEquals(string expectedTitle) {
        var actualTitle = Title;
        Assert.IsTrue(expectedTitle == actualTitle, $"Expected title '{expectedTitle}' but got '{actualTitle}'");
        return this;
    }

    public ITestObject Save() {
        AssertCanBeSaved();
        return new TestObject(DomainObject.Persist(AcceptanceTestCase.TestInvokeOptions(), DomainObject.GetPropertyMap()).Result, AcceptanceTestCase);
    }

    public ITestObject Refresh() => new TestObject(ROSIApi.GetObject(DomainObject.GetLinks().GetSelfLink()!.GetHref(), AcceptanceTestCase.TestInvokeOptions()).Result, AcceptanceTestCase);

    public ITestObject AssertIsType(Type expectedType) {
        var actual = DomainObject.GetDomainType();
        var expected = expectedType.FullName;

        Assert.IsTrue(actual == expected, $"Expected type {expected} but got {actual}");
        return this;
    }

    public ITestProperty GetPropertyByName(string name) {
        var q = Properties.Where(x => x.Name == name);
        if (!q.Any()) {
            Assert.Fail($"No Property named '{name}'");
        }

        if (q.Count() > 1) {
            Assert.Fail($"More than one Property named '{name}'");
        }

        return q.Single();
    }

    public ITestProperty GetPropertyById(string id) {
        var q = Properties.Where(x => x.Id == id);
        if (q.Count() != 1) {
            Assert.Fail($"No Property with Id '{id}'");
        }

        return q.Single();
    }

    public ITestObject AssertCanBeSaved() {
        Assert.IsTrue(IsTransient, $"Can only persist a transient object: {DomainObject.GetDomainType()}");

        try {
            DomainObject.ValidatePersist(AcceptanceTestCase.TestInvokeOptions()).Wait();
        }
        catch (ArgumentException ae) {
            if (ae.InnerException is HttpInvalidArgumentsRosiException hre) {
                var args = hre.Content?.GetMembers();
                if (args != null) {
                    foreach (var (name, argument) in args) {
                        if (argument.GetInvalidReason() is { } reason) {
                            Assert.Fail($"{name}:{reason}");
                        }
                    }
                }
            }

            Assert.Fail("Save Failed");
        }

        return this;
    }

    public ITestObject AssertCannotBeSaved() {
        try {
            AssertCanBeSaved();
        }
        catch (AssertFailedException) {
            // expected 
            return this;
        }

        Assert.Fail("Object should not be saveable");
        return this; // for compiler 
    }

    public ITestObject AssertIsTransient() {
        Assert.IsTrue(IsTransient, "Object is not transient");
        return this;
    }

    public ITestObject AssertIsPersistent() {
        Assert.IsTrue(IsPersistent, "Object is not persistent");
        return this;
    }

    public virtual string GetPropertyOrder() => string.Join(", ", Properties.Select(p => p.Name));

    public ITestHasProperties AssertPropertyOrderIs(string order) {
        Assert.AreEqual(order, GetPropertyOrder());
        return this;
    }

    private bool IsInteractionMode(string mode) => DomainObject.SafeGetExtension(ExtensionsApi.ExtensionKeys.x_ro_nof_interactionMode)?.ToString() == mode;
}