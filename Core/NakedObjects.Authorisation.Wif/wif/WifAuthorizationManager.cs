// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Configuration;
using System.IdentityModel.Configuration;
using System.Security.Claims;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Reflector.Security.Wif {
    public class WifAuthorizationManager : AuthorizationManagerAbstract, IAuthorizationManager {
        #region IAuthorizationManager Members

        public void UpdateAuthorizationCache(INakedObject nakedObject) {
            // do nothing 
        }

        #endregion

        private static ClaimsAuthorizationManager CreateManager() {
            var identityModelSection = (SystemIdentityModelSection) ConfigurationManager.GetSection("microsoft.identityModel");
            CustomTypeElement customTypeElement = identityModelSection.IdentityConfigurationElements.GetElement("").ClaimsAuthorizationManager;
            return CustomTypeElement.Resolve<ClaimsAuthorizationManager>(customTypeElement);
        }


        protected override void InitAuthorizer() {
            Authorizer = new WifAuthorizer(CreateManager());
        }
    }
}