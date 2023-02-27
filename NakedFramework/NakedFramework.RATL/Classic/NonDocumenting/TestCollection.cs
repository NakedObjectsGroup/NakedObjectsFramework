using System.Collections;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting; 

internal class TestCollection : ITestCollection {
    private readonly List collection;
    private readonly AcceptanceTestCase acceptanceTestCase;
    private IEnumerable<ITestObject> wrappedCollection;

    public IEnumerable<ITestObject> WrappedCollection {
        get {
            ITestObject GetTestObject(Link l) => new TestObject(ROSIApi.GetObject(l.GetHref(), acceptanceTestCase.TestInvokeOptions()).Result, acceptanceTestCase);

            return wrappedCollection ??= collection.GetValue().Select(GetTestObject).ToArray();
        }
    }

    public TestCollection(List collection, AcceptanceTestCase acceptanceTestCase) {
        this.collection = collection;
        this.acceptanceTestCase = acceptanceTestCase;
        
    }

    #region ITestCollection Members

    public string Title => throw new NotImplementedException();

    // public INakedObject NakedObject => collection;

    public ITestCollection AssertIsEmpty() {
        Assert.AreEqual(0, this.Count(), "Collection is not empty");
        return this;
    }

    public ITestCollection AssertIsNotEmpty() {
        Assert.IsTrue(this.Any(), "Collection is empty");
        return this;
    }

    public ITestCollection AssertCountIs(int count) {
        Assert.IsTrue(this.Count() == count, $"Collection Size is: {this.Count()} expected: {count}");
        return this;
    }

    public IEnumerator<ITestObject> GetEnumerator() {
        return WrappedCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public ITestCollection AssertIsTransient() {
       // Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Collection is not transient");
        return this;
    }

    public ITestCollection AssertIsPersistent() {
       // Assert.IsTrue(NakedObject.ResolveState.IsPersistent(), "Collection is not persistent");
        return this;
    }

    #endregion
}