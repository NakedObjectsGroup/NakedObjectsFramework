// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.Security {
    public interface IAuthorizer {
        void Init();

        void Shutdown();

        bool IsUsable(ISession session, INakedObject target, IIdentifier member);

        bool IsVisible(ISession session, INakedObject target, IIdentifier member);
    }
}