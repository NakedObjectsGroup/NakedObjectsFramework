// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Reflect;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NakedObjects.Meta.Test {
    public class CacheTest {
        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<IMenuBuilder, NullMenuBuilder>();
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IServicesConfiguration, ServicesConfiguration>();
        }

    
        [Test]
        public void JsonSerializeIntTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {typeof (int)}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            //Assert.AreEqual(10, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));

            var cache = container.Resolve<ISpecificationCache>();

            using (var fs = File.Open(@"c:\testmetadata\metadataint.json", FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw)) {
                jw.Formatting = Formatting.Indented;

                var serializer = new JsonSerializer {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};

                serializer.Serialize(jw, cache);
            }
        }

        [Test]
        public void BinarySerializeIntTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {typeof (int)}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            //Assert.AreEqual(10, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));

            var cache = container.Resolve<ISpecificationCache>();

            using (var fs = File.Open(@"c:\testmetadata\metadataint.bin", FileMode.OpenOrCreate)) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, cache);
            }

            // and roundtrip 

            ISpecificationCache newCache;

            using (var fs = File.Open(@"c:\testmetadata\metadataint.bin", FileMode.Open)) {
                IFormatter formatter = new BinaryFormatter();
                newCache = (ISpecificationCache)formatter.Deserialize(fs);
            }

            Assert.AreEqual(cache, newCache);

        }




        [Test]
        public void SoapSerializeIntTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] { typeof(int) }, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            //Assert.AreEqual(10, reflector.AllObjectSpecImmutables.Count());
            //Assert.AreSame(reflector.AllObjectSpecImmutables.First().Type, typeof(object));

            var cache = container.Resolve<ISpecificationCache>();

            using (var fs = File.Open(@"c:\testmetadata\metadataint.soap", FileMode.OpenOrCreate)) {
                IFormatter formatter = new SoapFormatter();
                formatter.Serialize(fs, cache);
            }
        }


        #region Nested type: NullMenuBuilder

        public class NullMenuBuilder : IMenuBuilder {
            #region IMenuBuilder Members

            public IMenu[] DefineMainMenus() {
                return new IMenu[] {};
            }

            #endregion
        }

        #endregion

        #region Nested type: SetWrapper

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

        #endregion

        #region Nested type: TestObjectWithByteArray

        public class TestObjectWithByteArray {
            public byte[] ByteArray { get; set; }
        }

        #endregion
    }
}