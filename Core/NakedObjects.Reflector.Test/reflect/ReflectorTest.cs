// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflect.Reflect {
    public class ReflectorTest {

        public class SetWrapper<T> : ISet<T> {
            private readonly ICollection<T> wrapped;

            public SetWrapper(ICollection<T> wrapped) {
                this.wrapped = wrapped;
            }

            #region ISet<T> Members

            public IEnumerator<T> GetEnumerator() {
                return wrapped.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            //public void ICollection<T>.Add(T item) {
            //   wrapped.Add(item);
            //}

            public void UnionWith(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other) {
                throw new NotImplementedException();
            }

            public bool Add(T item) {
                wrapped.Add(item);
                return true;
            }

            void ICollection<T>.Add(T item) {
                wrapped.Add(item);
            }

            public void Clear() {
                wrapped.Clear();
            }

            public bool Contains(T item) {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex) {
                throw new NotImplementedException();
            }

            public bool Remove(T item) {
                throw new NotImplementedException();
            }

            public int Count {
                get { return wrapped.Count; }
            }

            public bool IsReadOnly {
                get { return wrapped.IsReadOnly; }
            }

            #endregion
        }

        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>();
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();

            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IServicesConfiguration, ServicesConfiguration>();
        }

        [Test]
        public void ReflectNoTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        [Test]
        public void ReflectObjectType() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {typeof (object)}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(1, reflector.AllObjectSpecImmutables.Count());
            Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof (object));
        }

        [Test]
        public void ReflectListTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] { typeof(List<object>), typeof(List<int>) }, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(10, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));
        }

        [Test]
        public void ReflectSetTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] { typeof(SetWrapper<object>) }, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(16, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));
        }



        [Test]
        public void ReflectQueryableTypes() {
            var container = GetContainer();
            var qo = new List<object>() {}.AsQueryable();
            var qi = new List<int>() { }.AsQueryable();
            var rc = new ReflectorConfiguration(new Type[] { qo.GetType(), qi.GetType() }, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(9, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));
        }

        public class TestObjectWithByteArray {
            public byte[] ByteArray { get; set; }
        }

        [Test]
        public void ReflectByteArray() {
            var container = GetContainer();
           
            var rc = new ReflectorConfiguration(new Type[] { typeof(TestObjectWithByteArray) }, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
           // Assert.AreEqual(20, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));
        }

    }
}