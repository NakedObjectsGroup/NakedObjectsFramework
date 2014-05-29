// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.Security {
    public abstract class AuthorizationManagerAbstract {
        private IAuthorizer authorizer;

        protected internal virtual IAuthorizer Authorizer {
            set { authorizer = value; }
        }

        public virtual bool IsEditable(ISession session, INakedObject target, IIdentifier identifier) {
            return authorizer.IsUsable(session, target, identifier);
        }

        public virtual bool IsVisible(ISession session, INakedObject target, IIdentifier identifier) {
            return authorizer.IsVisible(session, target, identifier);
        }

        public virtual void Init() {
            InitAuthorizer();
            authorizer.Init();
        }

        public virtual void Shutdown() {
            authorizer.Shutdown();
        }

        protected internal abstract void InitAuthorizer();
    }
}