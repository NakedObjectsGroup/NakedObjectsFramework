// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Reflector.DotNet.Facets;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.Spec;
using NakedObjects.TestSystem;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public abstract class AbstractDotNetReflectorTest {
        protected DotNetSpecification specification;

        [SetUp]
        public virtual void SetUp() {
            var reflector = new DotNetReflector(new DefaultClassStrategy(), new FacetFactorySetImpl(), new FacetDecoratorSet()) {
                NonSystemServices = new INakedObject[] {}
            };

            NakedObjectsContext context = StaticContext.CreateInstance();
            context.SetReflector(reflector);
            context.SetObjectPersistor(new TestProxyPersistor());

            specification = LoadSpecification(reflector);
            specification.PopulateAssociatedActions(new INakedObject[]{});
        }

        protected abstract DotNetSpecification LoadSpecification(DotNetReflector reflector);
    }

    // Copyright (c) Naked Objects Group Ltd.
}