using System.Collections;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;
using static NakedFramework.RATL.Classic.Helpers.RATLHelpers;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestCollection : ITestCollection {
    private readonly AcceptanceTestCase acceptanceTestCase;
    private readonly IHasValue collection;
    private IEnumerable<ITestObject> wrappedCollection;

    public TestCollection(IHasValue collection, AcceptanceTestCase acceptanceTestCase) {
        this.collection = collection;
        this.acceptanceTestCase = acceptanceTestCase;
    }

    public IEnumerable<ITestObject> WrappedCollection => wrappedCollection ??= collection.GetValue().Select(link => GetTestObject(link, acceptanceTestCase)).ToArray();

    public string Title => throw new NotImplementedException();

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

    public IEnumerator<ITestObject> GetEnumerator() => WrappedCollection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ITestCollection AssertIsTransient() {
        throw new NotImplementedException();
        return this;
    }

    public ITestCollection AssertIsPersistent() {
        throw new NotImplementedException();
        return this;
    }
}