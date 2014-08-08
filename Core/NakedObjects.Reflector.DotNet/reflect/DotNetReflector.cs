// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class DotNetReflector : NakedObjectReflectorAbstract {
        public IClassStrategy ClassStrategy { get; set; }
        protected IFacetFactorySet FacetFactorySet { get; set; }
        public IntrospectionControlParameters IntrospectionControlParameters { get; private set; }

        /// <summary>
        ///     Initializes and wires up
        /// </summary>
        public override void Init() {
            base.Init();
            if (ClassStrategy == null) {
                ClassStrategy = new DefaultClassStrategy();
            }
            if (FacetFactorySet == null) {
                var facetFactorySetImpl = new FacetFactorySetImpl(this);
                facetFactorySetImpl.Init();
                FacetFactorySet = facetFactorySetImpl;
            }
            IntrospectionControlParameters = new IntrospectionControlParameters(FacetFactorySet, ClassStrategy);
            IgnoreCase = false;
        }

        protected override INakedObjectSpecification CreateSpecification(Type type) {
            return new DotNetSpecification(type, this);
        }

        /// <summary>
        ///     Used by the <see cref="DotNetIntrospector" /> created by the <see cref="DotNetSpecification" />
        ///     in <see cref="Install" />
        /// </summary>
        protected override NakedObjectSpecificationAbstract Install(Type type) {
            return new DotNetSpecification(type, this);
        }

        public override INakedObjectSpecification LoadSpecification(Type type) {
            return base.LoadSpecification(ClassStrategy.GetType(type));
        }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
        }

        public override IContainerInjector CreateContainerInjector(object[] services) {
            return new DotNetDomainObjectContainerInjector(this, services);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}