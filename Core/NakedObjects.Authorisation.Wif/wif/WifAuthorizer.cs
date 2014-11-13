// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Claims;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Reflect.Security.Wif {
    public class WifAuthorizer : IAuthorizer {
        private readonly ClaimsAuthorizationManager manager;

        public WifAuthorizer(ClaimsAuthorizationManager manager) {
            this.manager = manager;
        }

        #region IAuthorizer Members

        public bool IsVisible(ISession session, INakedObject target, IIdentifier member) {
            var checkViewType = member.IsField ? CheckType.ViewField : CheckType.Action;
            var checkEditType = member.IsField ? CheckType.EditField : CheckType.Action;

            var contextView = new AuthorizationContext((ClaimsPrincipal) session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int) checkViewType).ToString());
            var contextEdit = new AuthorizationContext((ClaimsPrincipal) session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int) checkEditType).ToString());

            // being editable implies visibility 
            return manager.CheckAccess(contextView) || manager.CheckAccess(contextEdit);
        }

        public bool IsUsable(ISession session, INakedObject target, IIdentifier member) {
            var checkType = member.IsField ? CheckType.EditField : CheckType.Action;
            var context = new AuthorizationContext((ClaimsPrincipal) session.Principal, member.ToIdentityString(IdentifierDepth.ClassName), ((int) checkType).ToString());

            return manager.CheckAccess(context);
        }

        #endregion

        
    }
}