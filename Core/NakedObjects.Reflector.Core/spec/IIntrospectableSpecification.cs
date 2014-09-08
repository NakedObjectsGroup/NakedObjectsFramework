// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.Spec {
    /// <summary>
    ///     Introduced to remove special-case processing for <see cref="INakedObjectSpecification" />s that
    ///     are not introspectable.
    /// </summary>
    public interface IIntrospectableSpecification : INakedObjectSpecification {
        /// <summary>
        ///     Discovers what attributes and behaviour the type specified by this specification. As specification are
        ///     cyclic (specifically a class will reference its subclasses, which in turn reference their superclass)
        ///     they need be created first, and then later work out its internals. This allows for cyclic references to
        ///     the be accommodated as there should always a specification available even though it might not be
        ///     complete.
        /// </summary>
        void Introspect(FacetDecoratorSet decorator);

        void PopulateAssociatedActions(Type[] services);
    }


    // Copyright (c) Naked Objects Group Ltd.
}