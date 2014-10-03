// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Peer {
    /// <summary>
    ///     Details about action and field members gained via reflection.
    /// </summary>
    public interface INakedObjectMemberPeer : IFacetHolder {
        IIntrospectableSpecification Specification { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}