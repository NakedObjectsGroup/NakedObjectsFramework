// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public interface INakedObjectReflector {
        bool IgnoreCase { get; }

        INakedObjectSpecification[] AllSpecifications { get; }

        INakedObject[] NonSystemServices { get; set; }

        IClassStrategy ClassStrategy { get; }

        IFacetFactorySet FacetFactorySet { get; }

        INakedObjectSpecification LoadSpecification(Type type);

        INakedObjectSpecification LoadSpecification(string name);

        void InstallServiceSpecifications(Type[] types);

        void PopulateContributedActions(Type[] services);

        void Shutdown();

        void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore);
    }

    // Copyright (c) Naked Objects Group Ltd.
}