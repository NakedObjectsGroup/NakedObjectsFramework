// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class AbstractFacetFactoryTest {
        protected Specification Specification;
        protected IMethodRemover MethodRemover;
        protected INakedObjectReflector Reflector;
        protected IMetamodelManager Metamodel;
        private Mock<IMethodRemover> mockMethodRemover;
        private Mock<INakedObjectReflector> mockReflector;
        private Mock<IMetamodelManager> mockMetadata;
        protected abstract Type[] SupportedTypes { get; }
        protected abstract IFacetFactory FacetFactory { get; }

        public virtual void SetUp() {
            Specification = new Specification();

            mockMethodRemover = new Mock<IMethodRemover>();
            mockReflector = new Mock<INakedObjectReflector>();
            mockMetadata = new Mock<IMetamodelManager>();

            MethodRemover = mockMethodRemover.Object;
            Reflector = mockReflector.Object;
            Metamodel = mockMetadata.Object;

            mockMethodRemover.Setup(remover => remover.RemoveMethod(It.IsAny<MethodInfo>()));
            mockMethodRemover.Setup(remover => remover.RemoveMethods(It.IsAny<IList<MethodInfo>>()));
        }

        public virtual void TearDown() {
            Specification = null;
            MethodRemover = null;
            Reflector = null;
        }


        protected static bool Contains<T>(T[] array, T val) {
            return array.Contains(val);
        }

        protected static MethodInfo FindMethod(Type type, string methodName, Type[] parameterTypes) {
            try {
                return type.GetMethod(methodName, parameterTypes);
            }
            catch (AmbiguousMatchException) {
                return null;
            }
            catch (ArgumentNullException) {
                return null;
            }
            catch (ArgumentException) {
                return null;
            }
        }

        protected static MethodInfo FindMethodIgnoreParms(Type type, string methodName) {
            try {
                return type.GetMethod(methodName);
            }
            catch (AmbiguousMatchException) {
                return null;
            }
            catch (ArgumentNullException) {
                return null;
            }
            catch (ArgumentException) {
                return null;
            }
        }


        protected static PropertyInfo FindProperty(Type type, string propertyName) {
            try {
                return type.GetProperty(propertyName);
            }
            catch (AmbiguousMatchException) {
                return null;
            }
            catch (ArgumentNullException) {
                return null;
            }
            catch (ArgumentException) {
                return null;
            }
        }

        protected MethodInfo FindMethod(Type type, string methodName) {
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