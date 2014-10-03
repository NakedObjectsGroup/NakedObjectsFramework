// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Peer {
    public interface INakedObjectActionPeer : INakedObjectMemberPeer, IOrderableElement<INakedObjectActionPeer> {
        INakedObjectActionParamPeer[] Parameters { get; }
        bool IsFinderMethod { get;  }
        IIntrospectableSpecification ReturnType { get; }
        bool IsContributedTo(IIntrospectableSpecification introspectableSpecification);
    }

    // Copyright (c) Naked Objects Group Ltd.
}