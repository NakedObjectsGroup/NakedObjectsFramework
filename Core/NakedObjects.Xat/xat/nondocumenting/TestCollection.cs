// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;


namespace NakedObjects.Xat {
    internal class TestCollection : ITestCollection {
        private readonly INakedObject collection;
        private readonly IEnumerable<ITestObject> wrappedCollection;

        public TestCollection(INakedObject collection, ITestObjectFactory factory, INakedObjectManager manager) {
            this.collection = collection;
            wrappedCollection = collection.GetAsEnumerable(manager).Select(factory.CreateTestObject);
        }

        #region ITestCollection Members

        public string Title {
            get { return collection.TitleString(); }
        }

        public INakedObject NakedObject {
            get { return collection; }
        }

        public ITestCollection AssertIsEmpty() {
            Assert.AreEqual(0, this.Count(), "Collection is not empty");
            return this;
        }

        public ITestCollection AssertIsNotEmpty() {
            Assert.IsTrue(this.Any(), "Collection is empty");
            return this;
        }

        public ITestCollection AssertCountIs(int count) {
            Assert.IsTrue(this.Count() == count, string.Format("Collection Size is: {0} expected: {1}", this.Count(), count));
            return this;
        }

        public IEnumerator<ITestObject> GetEnumerator() {
           return wrappedCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public ITestCollection AssertIsTransient() {
            Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Collection is not transient");
            return this;
        }

        public ITestCollection AssertIsPersistent() {
            Assert.IsTrue(NakedObject.ResolveState.IsPersistent(), "Collection is not persistent");
            return this;
        }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}