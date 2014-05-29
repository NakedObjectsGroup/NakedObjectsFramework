// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Claims;
using System.Security.Principal;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.Security.Wif {
    public class WifAuthorizer : IAuthorizer {
        private readonly ClaimsAuthorizationManager manager;

        public WifAuthorizer(ClaimsAuthorizationManager manager) {
            this.manager = manager;
        }

        #region IAuthoriser Members

        public void Init() {
            // do nothing
        }

        public void Shutdown() {
            // do nothing
        }

        public bool IsVisible(ISession session, INakedObject target, IIdentifier member) {
            var checkViewType = member.IsField ? CheckType.ViewField : CheckType.Action;
            var checkEditType = member.IsField ? CheckType.EditField : CheckType.Action;

            var contextView = new AuthorizationContext((ClaimsPrincipal)session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int)checkViewType).ToString());
            var contextEdit = new AuthorizationContext((ClaimsPrincipal)session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int)checkEditType).ToString());

            // being editable implies visibility 
            return manager.CheckAccess(contextView) || manager.CheckAccess(contextEdit);
        }

        public bool IsUsable(ISession session, INakedObject target, IIdentifier member) {
            var checkType = member.IsField ? CheckType.EditField : CheckType.Action;
            var context = new AuthorizationContext((ClaimsPrincipal)session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int)checkType).ToString());

            return manager.CheckAccess(context);
        }

        #endregion
    }
}