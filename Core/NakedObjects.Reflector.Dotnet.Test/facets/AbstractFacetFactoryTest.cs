// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class AbstractFacetFactoryTest {
        protected FacetHolderImpl facetHolder;
        protected IMethodRemover methodRemover;
        protected INakedObjectReflector reflector;
        protected abstract Type[] SupportedTypes { get; }
        protected abstract IFacetFactory FacetFactory { get; }

        public virtual void SetUp() {
            facetHolder = new FacetHolderImpl();
            methodRemover = new ProgrammableMethodRemover();
            reflector = new Mock<INakedObjectReflector>().Object;
        }

        public virtual void TearDown() {
            facetHolder = null;
            methodRemover = null;
            reflector = null;
        }


        protected static bool Contains<T>(T[] array, T val) {
            foreach (T t in array) {
                if (t.Equals(val)) {
                    return true;
                }
            }
            return false;
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

        protected void AssertNoMethodsRemoved() {
            //Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Count == 0);
            //Assert.IsTrue(methodRemover.GetRemoveMethodArgsCalls().Count == 0);
            Assert.Fail(); // fix this 
        }

        public abstract void TestFeatureTypes();
    }

    // Copyright (c) Naked Objects Group Ltd.
}