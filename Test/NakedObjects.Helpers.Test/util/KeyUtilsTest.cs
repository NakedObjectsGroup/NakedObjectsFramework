// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Util;
using NakedObjects.UtilInternal;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects {
    [TestClass]
    public class KeyUtilsTest {
        #region Test classes ComponentModel.DataAnnotations.Key 

        public class TestContainer : IDomainObjectContainer, IInternalAccess {
            #region IDomainObjectContainer Members

            #region not impl

            public IPrincipal Principal {
                get { throw new NotImplementedException(); }
            }

            public void Refresh(object obj) {
                throw new NotImplementedException();
            }

            public void Resolve(object obj) {
                throw new NotImplementedException();
            }

            public void Resolve(object obj, object field) {
                throw new NotImplementedException();
            }

            public void ObjectChanged(object obj) {
                throw new NotImplementedException();
            }

            public T NewTransientInstance<T>() where T : new() {
                throw new NotImplementedException();
            }

            public T NewViewModel<T>() where T : IViewModel, new() {
                throw new NotImplementedException();
            }

            public IViewModel NewViewModel(Type type) {
                throw new NotImplementedException();
            }

            public object NewTransientInstance(Type type) {
                throw new NotImplementedException();
            }

            public bool IsPersistent(object obj) {
                throw new NotImplementedException();
            }

            public void Persist<T>(ref T transientObject) {
                throw new NotImplementedException();
            }

            public void DisposeInstance(object persistentObject) {
                throw new NotImplementedException();
            }

            public void InformUser(string message) {
                throw new NotImplementedException();
            }

            public void WarnUser(string message) {
                throw new NotImplementedException();
            }

            public void RaiseError(string message) {
                throw new NotImplementedException();
            }


            public IQueryable Instances(Type type) {
                throw new NotImplementedException();
            }

            public void AbortCurrentTransaction() {
                throw new NotImplementedException();
            }

            public T NewPersistentInstance<T>() {
                throw new NotImplementedException();
            }

            public object NewPersistentInstance(Type type) {
                throw new NotImplementedException();
            }

            public T NewInstance<T>(object sameStateAs) {
                throw new NotImplementedException();
            }

            public void MakePersistent(ref object transientObject) {
                throw new NotImplementedException();
            }

            #endregion

            public IQueryable<T> Instances<T>() where T : class {
                if (typeof (T) == typeof (TestKey)) {
                    return new[] {new TestKey {AName = 1}}.Cast<T>().AsQueryable();
                }
                if (typeof (T) == typeof (TestStringKey)) {
                    return new[] {new TestStringKey {AName = "aName"}}.Cast<T>().AsQueryable();
                }

                return null;
            }

            #endregion

            #region IInternalAccess Members

            public PropertyInfo[] GetKeys(Type type) {
                return type.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToArray();
            }

            public object FindByKeys(Type type, object[] keys) {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class TestKey {
            [Key]
            public int AName { get; set; }
        }

        public class TestMultiKey {
            [Key]
            public int AName { get; set; }

            [Key]
            public int AName1 { get; set; }
        }

        public class TestNoKey {
            public int AName { get; set; }
        }

        public class TestStringKey {
            [Key]
            public string AName { get; set; }
        }

        #endregion

        #region tests ComponentModel.DataAnnotations.Key 

        [TestMethod]
        public void TestGetKeys() {
            var container = new TestContainer();
            PropertyInfo[] keys = container.GetKeys(typeof (TestKey));
            Assert.AreEqual(1, keys.Count());
            Assert.AreSame(typeof (TestKey).GetProperty("AName"), keys.Single());
        }

        [TestMethod]
        public void TestGetNoKeys() {
            var container = new TestContainer();
            PropertyInfo[] keys = container.GetKeys(typeof (TestNoKey));
            Assert.AreEqual(0, keys.Count());
        }

        [TestMethod]
        public void TestGetMultiKeys() {
            var container = new TestContainer();
            PropertyInfo[] keys = container.GetKeys(typeof (TestMultiKey));
            Assert.AreEqual(2, keys.Count());
            Assert.AreSame(typeof (TestMultiKey).GetProperty("AName"), keys.First());
            Assert.AreSame(typeof (TestMultiKey).GetProperty("AName1"), keys.Last());
        }

        [TestMethod]
        public void TestGetSingleKey() {
            var container = new TestContainer();
            PropertyInfo key = container.GetSingleKey(typeof (TestKey));
            Assert.AreSame(typeof (TestKey).GetProperty("AName"), key);
        }

        [TestMethod]
        public void TestGetSingleNoKey() {
            var container = new TestContainer();

            try {
                PropertyInfo key = container.GetSingleKey(typeof (TestNoKey));
                Assert.Fail("Exception expected");
            }
            catch (DomainException) {
                // expected
            }
        }

        [TestMethod]
        public void TestGetSingleMultiKey() {
            var container = new TestContainer();
            try {
                PropertyInfo key = container.GetSingleKey(typeof (TestMultiKey));
                Assert.Fail("Exception expected");
            }
            catch (DomainException) {
                // expected
            }
        }

        #endregion
    }
}