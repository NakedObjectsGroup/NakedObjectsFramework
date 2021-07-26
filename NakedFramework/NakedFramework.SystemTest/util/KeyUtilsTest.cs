// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedFramework;
using NakedFramework.Error;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedObjects.SystemTest.Util {
    [TestFixture]
    public class KeyUtilsTest {
        [Test]
        public void TestGetKeys() {
            var container = new TestContainer();
            var keys = container.GetKeys(typeof(TestKey));
            Assert.AreEqual(1, keys.Length);
            Assert.AreSame(typeof(TestKey).GetProperty("AName"), keys.Single());
        }

        [Test]
        public void TestGetMultiKeys() {
            var container = new TestContainer();
            var keys = container.GetKeys(typeof(TestMultiKey));
            Assert.AreEqual(2, keys.Length);
            Assert.AreSame(typeof(TestMultiKey).GetProperty("AName"), keys.First());
            Assert.AreSame(typeof(TestMultiKey).GetProperty("AName1"), keys.Last());
        }

        [Test]
        public void TestGetNoKeys() {
            var container = new TestContainer();
            var keys = container.GetKeys(typeof(TestNoKey));
            Assert.AreEqual(0, keys.Length);
        }

        [Test]
        public void TestGetSingleKey() {
            var container = new TestContainer();
            var key = container.GetSingleKey(typeof(TestKey));
            Assert.AreSame(typeof(TestKey).GetProperty("AName"), key);
        }

        [Test]
        public void TestGetSingleMultiKey() {
            var container = new TestContainer();
            try {
                var key = container.GetSingleKey(typeof(TestMultiKey));
                Assert.Fail("Exception expected");
            }
            catch (DomainException) {
                // expected
            }
        }

        [Test]
        public void TestGetSingleNoKey() {
            var container = new TestContainer();

            try {
                var key = container.GetSingleKey(typeof(TestNoKey));
                Assert.Fail("Exception expected");
            }
            catch (DomainException) {
                // expected
            }
        }

        public class TestContainer : IDomainObjectContainer {
            #region IDomainObjectContainer Members

            public IQueryable<T> Instances<T>() where T : class {
                if (typeof(T) == typeof(TestKey)) {
                    return new[] {new TestKey {AName = 1}}.Cast<T>().AsQueryable();
                }

                if (typeof(T) == typeof(TestStringKey)) {
                    return new[] {new TestStringKey {AName = "aName"}}.Cast<T>().AsQueryable();
                }

                return null;
            }

            #endregion

            public PropertyInfo[] GetKeys(Type type) {
                return type.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToArray();
            }

            public object FindByKeys(Type type, object[] keys) => throw new NotImplementedException();

            #region not impl

            public T GetService<T>() => throw new NotImplementedException();

            public IPrincipal Principal => throw new NotImplementedException();

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

            public T NewTransientInstance<T>() where T : new() => throw new NotImplementedException();

            public T NewViewModel<T>() where T : IViewModel, new() => throw new NotImplementedException();

            public IViewModel NewViewModel(Type type) => throw new NotImplementedException();

            public object NewTransientInstance(Type type) => throw new NotImplementedException();

            public bool IsPersistent(object obj) => throw new NotImplementedException();

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

            public IQueryable Instances(Type type) => throw new NotImplementedException();

            public void AbortCurrentTransaction() {
                throw new NotImplementedException();
            }

            public T NewPersistentInstance<T>() => throw new NotImplementedException();

            public object NewPersistentInstance(Type type) => throw new NotImplementedException();

            public T NewInstance<T>(object sameStateAs) => throw new NotImplementedException();

            public void MakePersistent(ref object transientObject) {
                throw new NotImplementedException();
            }

            public ITitleBuilder NewTitleBuilder() => throw new NotImplementedException();

            public ITitleBuilder NewTitleBuilder(string text) => throw new NotImplementedException();

            public ITitleBuilder NewTitleBuilder(object obj, string defaultTitle = null) => throw new NotImplementedException();

            public string TitleOf(object obj, string format = null) => throw new NotImplementedException();

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
    }
}