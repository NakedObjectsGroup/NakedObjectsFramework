using System.Collections.Generic;

namespace NakedObjects.Xat.Generic {
    public interface ITestCollection<TOwner, TReturn> : ITestNaked, IEnumerable<ITestObject<TReturn>> {
        ITestCollection<TOwner, TReturn> AssertIsEmpty();

        ITestCollection<TOwner, TReturn> AssertIsNotEmpty();

        ITestCollection<TOwner, TReturn> AssertCountIs(int count);
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}