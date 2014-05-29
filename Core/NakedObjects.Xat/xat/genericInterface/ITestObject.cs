using System;

namespace NakedObjects.Xat.Generic {
    public interface ITestObject<T> : ITestObject, ITestHasActions<T>, ITestHasProperties<T> {
        new ITestObject<T> AssertIsImmutable();

        new ITestObject<T> AssertIsDescribedAs(string expectedDescription);

        new ITestObject<T> AssertTitleEquals(string expectedTitle);

        new ITestObject<T> AssertIsType(Type expectedType);

        new T GetDomainObject();

        new string IconName { get; }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}