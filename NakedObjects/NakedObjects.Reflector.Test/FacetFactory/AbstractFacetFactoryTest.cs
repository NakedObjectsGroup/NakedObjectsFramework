// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.DependencyInjection.Component;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.Test.FacetFactory {
    public abstract class AbstractFacetFactoryTest {
        protected IClassStrategy ClassStrategy = new ObjectClassStrategy(null);
        private ILogger<AbstractFacetFactoryTest> logger;
        protected ILoggerFactory LoggerFactory;
        protected IMetamodelManager Metamodel;
        protected IMethodRemover MethodRemover;
        private Mock<IMetamodelManager> mockMetadata;
        private Mock<IMethodRemover> mockMethodRemover;
        private Mock<IReflector> mockReflector;

        protected IReflector Reflector;
        protected ISpecificationBuilder Specification;
        protected abstract Type[] SupportedTypes { get; }
        protected abstract IFacetFactory FacetFactory { get; }

        public virtual void SetUp() {
            Specification = new TestSpecification();

            mockMethodRemover = new Mock<IMethodRemover>();
            mockReflector = new Mock<IReflector>();
            mockMetadata = new Mock<IMetamodelManager>();
            LoggerFactory = new LoggerFactory();
            logger = LoggerFactory.CreateLogger<AbstractFacetFactoryTest>();

            MethodRemover = mockMethodRemover.Object;
            Reflector = mockReflector.Object;
            Metamodel = mockMetadata.Object;

            mockMethodRemover.Setup(remover => remover.RemoveMethod(It.IsAny<MethodInfo>()));
            mockMethodRemover.Setup(remover => remover.RemoveMethods(It.IsAny<IList<MethodInfo>>()));

            mockReflector.Setup(reflector =>
                                    reflector.LoadSpecification(It.IsAny<Type>(), It.IsAny<IImmutableDictionary<string, ITypeSpecBuilder>>())).Returns((Type t, IImmutableDictionary<string, ITypeSpecBuilder> m) => (null, m));

            mockReflector.Setup(reflector =>
                                    reflector.LoadSpecification<ITypeSpecBuilder>(It.IsAny<Type>(), It.IsAny<IImmutableDictionary<string, ITypeSpecBuilder>>())).Returns((Type t, IImmutableDictionary<string, ITypeSpecBuilder> m) => (null, m));

            mockReflector.Setup(reflector =>
                                    reflector.LoadSpecification<IObjectSpecBuilder>(It.IsAny<Type>(), It.IsAny<IImmutableDictionary<string, ITypeSpecBuilder>>())).Returns((Type t, IImmutableDictionary<string, ITypeSpecBuilder> m) => (null, m));

            mockReflector.Setup(reflector => reflector.ClassStrategy).Returns(ClassStrategy);
        }

        public virtual void TearDown() {
            Specification = null;
            MethodRemover = null;
            Reflector = null;
        }

        protected IFacetFactoryOrder<T> GetOrder<T>()
        {
            return new TestFacetFactoryOrder<T>(ObjectFacetFactories.StandardFacetFactories());
        }

        protected static bool Contains<T>(T[] array, T val)
        {
            return array.Contains(val);
        }

        protected MethodInfo FindMethod(Type type, string methodName, Type[] parameterTypes) {
            try {
                return type.GetMethod(methodName, parameterTypes);
            }
            catch (AmbiguousMatchException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentNullException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
        }

        protected MethodInfo FindMethodIgnoreParms(Type type, string methodName) {
            try {
                return type.GetMethod(methodName);
            }
            catch (AmbiguousMatchException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentNullException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentException) {
                logger.LogWarning($"Failed to find method {methodName} on type {type.FullName}");
                return null;
            }
        }

        protected PropertyInfo FindProperty(Type type, string propertyName) {
            try {
                return type.GetProperty(propertyName);
            }
            catch (AmbiguousMatchException) {
                logger.LogWarning($"Failed to find property {propertyName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentNullException) {
                logger.LogWarning($"Failed to find property {propertyName} on type {type.FullName}");
                return null;
            }
            catch (ArgumentException) {
                logger.LogWarning($"Failed to find property {propertyName} on type {type.FullName}");
                return null;
            }
        }

        protected MethodInfo FindMethod(Type type, string methodName)
        {
            return FindMethod(type, methodName, Type.EmptyTypes);
        }

        protected void AssertRemovedCalled(int count) {
            mockMethodRemover.Verify(remover => remover.RemoveMethod(It.IsAny<MethodInfo>()), Times.Exactly(count));
        }

        protected void AssertNoMethodsRemoved() {
            mockMethodRemover.Verify(remover => remover.RemoveMethod(It.IsAny<MethodInfo>()), Times.Never);
            mockMethodRemover.Verify(remover => remover.RemoveMethods(It.IsAny<IList<MethodInfo>>()), Times.Never);
        }

        protected void AssertMethodRemoved(MethodInfo mi) {
            mockMethodRemover.Verify(remover => remover.RemoveMethod(It.Is<MethodInfo>(i => i == mi)), Times.AtLeastOnce);
        }

        protected void AssertMethodsRemoved(MethodInfo[] mis) {
            mockMethodRemover.Verify(remover => remover.RemoveMethods(It.Is<IList<MethodInfo>>(i => i.SequenceEqual(mis))), Times.AtLeastOnce);
        }

        protected void AssertMethodNotRemoved(MethodInfo mi) {
            mockMethodRemover.Verify(remover => remover.RemoveMethod(It.Is<MethodInfo>(i => i == mi)), Times.Never);
        }

        public abstract void TestFeatureTypes();
    }

    // Copyright (c) Naked Objects Group Ltd.
}