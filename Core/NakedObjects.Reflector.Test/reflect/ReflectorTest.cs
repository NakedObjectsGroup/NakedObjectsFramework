// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.Reflect {
    public class ReflectorTest {
        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>();
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();

            container.RegisterType<IReflector, DotNet.Reflect.Reflector>();
            container.RegisterType<IMetamodel, DotNet.Reflect.Metamodel>();
            container.RegisterType<IMetamodelMutable, DotNet.Reflect.Metamodel>();
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

    }
}