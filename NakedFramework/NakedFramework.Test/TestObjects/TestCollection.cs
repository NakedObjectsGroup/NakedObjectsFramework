// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Util;
using NakedFramework.Test.Interface;

namespace NakedFramework.Test.TestObjects; 

internal class TestCollection : ITestCollection {
    private readonly IEnumerable<ITestObject> wrappedCollection;

    public TestCollection(INakedObjectAdapter collection, ITestObjectFactory factory, INakedObjectManager manager) {
        NakedObject = collection;
        wrappedCollection = collection.GetAsEnumerable(manager).Select(factory.CreateTestObject);
    }

    #region ITestCollection Members

    public string Title => NakedObject.TitleString();

    public INakedObjectAdapter NakedObject { get; }

    public ITestCollection AssertCountIs(int count) {
        Assert.IsTrue(this.Count() == count, $"Collection Size is: {this.Count()} expected: {count}");
        return this;
    }

    public IEnumerator<ITestObject> GetEnumerator() => wrappedCollection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.