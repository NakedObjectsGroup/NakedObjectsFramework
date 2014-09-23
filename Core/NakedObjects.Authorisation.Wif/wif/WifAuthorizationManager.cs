// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Configuration;
using System.IdentityModel.Configuration;
using System.Security.Claims;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.Security.Wif {
    public class WifAuthorizationManager : AuthorizationManagerAbstract, IAuthorizationManager {
        #region IAuthorizationManager Members

        public bool IsVisible(ISession session, ILifecycleManager persistor, INakedObject target, IIdentifier identifier) {
            throw new System.NotImplementedException();
        }

        public bool IsEditable(ISession session, ILifecycleManager persistor, INakedObject target, IIdentifier identifier) {
            throw new System.NotImplementedException();
        }

        public void UpdateAuthorizationCache(INakedObject nakedObject) {
            // do nothing 
        }

        #endregion

        private static ClaimsAuthorizationManager CreateManager() {
            var identityModelSection = (SystemIdentityModelSection) ConfigurationManager.GetSection("microsoft.identityModel");
            CustomTypeElement customTypeElement = identityModelSection.IdentityConfigurationElements.GetElement("").ClaimsAuthorizationManager;
            return CustomTypeElement.Resolve<ClaimsAuthorizationManager>(customTypeElement);
        }


        protected  void InitAuthorizer() {
            Authorizer = new WifAuthorizer(CreateManager());
        }
    }
}