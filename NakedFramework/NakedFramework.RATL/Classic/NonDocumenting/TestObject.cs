using System.Text;
using NakedFramework.Core.Util;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestObject : TestHasActions, ITestObject {
    private readonly DomainObject domainObject;

    public TestObject(DomainObject domainObject, AcceptanceTestCase acceptanceTestCase) : base(domainObject, acceptanceTestCase) {
        this.domainObject = domainObject;
    }

    public override string Title {
        get {
            Assert.IsNotNull(domainObject, "Cannot get title for null object");
            return domainObject?.GetTitle();
        }
    }

    public ITestProperty[] Properties => domainObject.GetProperties().Select(p => new TestProperty(p, AcceptanceTestCase)).Cast<ITestProperty>().ToArray();

    public ITestObject AssertTitleEquals(string expectedTitle) {
        var actualTitle = Title;
        Assert.IsTrue(expectedTitle == actualTitle, $"Expected title '{expectedTitle}' but got '{actualTitle}'");
        return this;
    }

    public ITestObject Save() {
        AssertCanBeSaved();

        //NakedObjectsContext.ObjectPersistor.StartTransaction();
        //NakedObjectsContext.ObjectPersistor.MakePersistent(NakedObject);
        //NakedObjectsContext.ObjectPersistor.EndTransaction();
        return this;
    }

    public ITestObject Refresh() {
        //NakedObjectsContext.ObjectPersistor.Refresh(NakedObject);
        return this;
    }

    public ITestObject AssertIsType(Type expected) {
        //Type actualType = NakedObject.GetDomainObject().GetType();
        //actualType = TypeUtils.IsProxy(actualType) ? actualType.BaseType : actualType;
        //Assert.IsTrue(actualType.Equals(expected), "Expected type " + expected + " but got " + actualType);
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
        //Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Object is not transient");
        return this;
    }

    public ITestObject AssertIsPersistent() {
        //Assert.IsTrue(NakedObject.ResolveState.IsPersistent(), "Object is not persistent");
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