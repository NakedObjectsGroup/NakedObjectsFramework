// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Util;
using System.Text;


namespace NakedObjects.Xat {
    internal class TestObject : TestHasActions, ITestObject {
        private readonly ILifecycleManager persistor;
        private static readonly ILog LOG;

        static TestObject() {
            LOG = LogManager.GetLogger(typeof (TestObject));
        }

        public TestObject(ILifecycleManager persistor,   INakedObject nakedObject, ITestObjectFactory factory)
            : base(factory, persistor) {
            this.persistor = persistor;
            LOG.DebugFormat("Created test object for {0}", nakedObject);
            NakedObject = nakedObject;
        }

        #region ITestObject Members

        public ITestProperty[] Properties {
            get { return NakedObject.Specification.Properties.Select(x => factory.CreateTestProperty(x, this)).ToArray(); }
        }

        public object GetDomainObject() {
            return NakedObject.GetDomainObject();
        }

        public string IconName {
            get {
                Assert.IsNotNull(NakedObject, "Cannot get icon for null object");
                return NakedObject.IconName();
            }
        }

        public override string Title {
            get {
                Assert.IsNotNull(NakedObject, "Cannot get title for null object");
                return NakedObject.TitleString();
            }
        }

        public ITestObject Save() {
            AssertCanBeSaved();

            persistor.StartTransaction();
            persistor.MakePersistent(NakedObject);
            persistor.EndTransaction();
            return this;
        }

        public ITestObject Refresh() {
            persistor.Refresh(NakedObject);
            return this;
        }

        public ITestObject AssertTitleEquals(string expectedTitle) {
            Assert.IsTrue(Title.Equals(expectedTitle), "Expected title '" + expectedTitle + "' but got '" + Title + "'");

            return this;
        }

        public ITestObject AssertIsType(Type expected) {
            Type actualType = NakedObject.GetDomainObject().GetType();
            actualType = TypeUtils.IsProxy(actualType) ? actualType.BaseType : actualType;
            Assert.IsTrue(actualType.Equals(expected), "Expected type " + expected + " but got " + actualType);
            return this;
        }

        public ITestProperty GetPropertyByName(string name) {
            var q = Properties.Where(x => x.Name == name);
            if (q.Count() < 1) Assert.Fail("No Property named '" + name + "'");
            if (q.Count() > 1) Assert.Fail("More than one Property named '" + name + "'");
            return q.Single();
        }

        public ITestProperty GetPropertyById(string id) {
            var q = Properties.Where(x => x.Id == id);
            if (q.Count() != 1) Assert.Fail("No Property with Id '" + id + "'");
            return q.Single();
        }

        public ITestObject AssertCanBeSaved() {
            Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Can only persist a transient object: " + NakedObject);
            Assert.IsTrue(NakedObject.Specification.Persistable == PersistableType.UserPersistable, "Object not persistable by user: " + NakedObject);

            Properties.ForEach(p => p.AssertIsValidToSave());


            INakedObjectValidation[] validators = NakedObject.Specification.ValidateMethods();

            foreach (INakedObjectValidation validator in validators) {
                string[] parmNames = validator.ParameterNames;

                List<ITestProperty> matchingparms = parmNames.Select(pn => Properties.Single(t => t.Id.ToLower() == pn)).ToList();

                if (matchingparms.Count() == parmNames.Count()) {
                    string result = validator.Execute(NakedObject, matchingparms.Select(t => t.Content != null ? t.Content.NakedObject : null).ToArray());

                    if (!string.IsNullOrEmpty(result)) {
                        Assert.Fail(result);
                    }
                }
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
            Assert.IsTrue(NakedObject.ResolveState.IsTransient(), "Object is not transient");
            return this;
        }

        public ITestObject AssertIsPersistent() {
            Assert.IsTrue(NakedObject.ResolveState.IsPersistent(), "Object is not persistent");
            return this;
        }

        public virtual string GetPropertyOrder()
        {
            var props = this.Properties;
            var order = new StringBuilder();
            for (int i = 0; i < props.Length; i++)
            {
                order.Append(props[i].Name);
                order.Append(i < props.Length - 1 ? ", " : "");
            }
            return order.ToString();
        }

        public ITestHasProperties AssertPropertyOrderIs(string order)
        {
            Assert.AreEqual(order, GetPropertyOrder());
            return this;
        }

        #endregion

        public override bool Equals(Object obj) {
            if (obj is TestObject) {
                var testObject = (TestObject) obj;
                return testObject.NakedObject == NakedObject;
            }
            return false;
        }

        public override string ToString() {
            return NakedObject == null ? "" : NakedObject.ToString();
        }

        public override int GetHashCode() {
            return NakedObject.GetHashCode();
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}