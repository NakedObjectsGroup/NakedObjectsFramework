// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public interface INakedObjectReflector {
        bool IgnoreCase { get; }

        INakedObjectSpecification[] AllSpecifications { get; }

        /// <summary>
        ///     Provided so that the <see cref="INakedObjectPersistor" /> can lookup if wasn't explicitly injected into
        ///     the persistor via other means.
        /// </summary>
        /// <para>
        ///     Indicates to the component that it is to initialise itself as it will soon be receiving requests
        /// </para>
        void Init();

        INakedObjectSpecification LoadSpecification(Type type);

        INakedObjectSpecification LoadSpecification(string name);

        void InstallServiceSpecifications(Type[] types);

        void PopulateContributedActions(INakedObject[] services);

        /// <summary>
        ///     Indicates to the component that no more requests will be made of it and it can safely release any
        ///     services it has hold of.
        /// </summary>
        void Shutdown();

       
    }

    // Copyright (c) Naked Objects Group Ltd.
}