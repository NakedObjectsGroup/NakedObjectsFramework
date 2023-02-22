using System.Text;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestObject : TestHasActions, ITestObject {
    public TestObject(DomainObject domainObject, AcceptanceTestCase acceptanceTestCase) : base(domainObject, acceptanceTestCase) { }

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

    private Dictionary<string, object> GetPropertyMap() {
        return DomainObject.GetProperties().ToDictionary(p => p.GetId(), p => p.GetValue() ?? p.GetLinkValue());
    }

    public ITestObject Save() {
        AssertCanBeSaved();
        DomainObject.Persist(AcceptanceTestCase.TestInvokeOptions(), GetPropertyMap()); 


        //NakedObjectsContext.ObjectPersistor.StartTransaction();
        //NakedObjectsContext.ObjectPersistor.MakePersistent(NakedObject);
        //NakedObjectsContext.ObjectPersistor.EndTransaction();
        return this;
    }

    public ITestObject Refresh() =>
        //NakedObjectsContext.ObjectPersistor.Refresh(NakedObject);
        this;

    public ITestObject AssertIsType(Type expected) =>
        //Type actualType = NakedObject.GetDomainObject().GetType();
        //actualType = TypeUtils.IsProxy(actualType) ? actualType.BaseType : actualType;
        //Assert.IsTrue(actualType.Equals(expected), "Expected type " + expected + " but got " + actualType);
        this;

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


        //Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Can only persist a transient object: " + NakedObject);
        //Assert.IsTrue(NakedObject.Specification.Persistable == Persistable.USER_PERSISTABLE, "Object not persistable by user: " + NakedObject);
        //Properties.ForEach(p => p.AssertIsValidToSave());
        //INakedObjectValidation[] validators = NakedObject.Specification.ValidateMethods();
        //foreach (INakedObjectValidation validator in validators) {
        //    string[] parmNames = validator.ParameterNames;
        //    var matchingparms = parmNames.Select(pn => Properties.Single(t => t.Id.ToLower() == pn)).ToList();
        //    if (matchingparms.Count() == parmNames.Count()) {
        //        string result = validator.Execute(NakedObject, matchingparms.Select(t => t.Content != null ? t.Content.NakedObject : null).ToArray());
        //        if (!string.IsNullOrEmpty(result)) {
        //            Assert.Fail(result);
        //        }
        //    }
        //}
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
        var isTransient = DomainObject.SafeGetExtension(ExtensionsApi.ExtensionKeys.x_ro_nof_interactionMode)?.ToString() == "transient";
        Assert.IsTrue(isTransient, "Object is not transient");
        return this;
    }

    public ITestObject AssertIsPersistent() {
        var isPersistent = DomainObject.SafeGetExtension(ExtensionsApi.ExtensionKeys.x_ro_nof_interactionMode)?.ToString() == "persistent";
        Assert.IsTrue(isPersistent, "Object is not persistent");
        return this;
    }

    public virtual string GetPropertyOrder() {
        var props = Properties;
        var order = new StringBuilder();
        for (var i = 0; i < props.Length; i++) {
            order.Append(props[i].Name);
            order.Append(i < props.Length - 1 ? ", " : "");
        }

        return order.ToString();
    }

    public ITestHasProperties AssertPropertyOrderIs(string order) {
        Assert.AreEqual(order, GetPropertyOrder());
        return this;
    }
}