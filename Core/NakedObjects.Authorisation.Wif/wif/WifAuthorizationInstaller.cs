// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Security.Wif {
    public class WifAuthorizationInstaller : IAuthorizerInstaller {
        #region IAuthorizerInstaller Members

        public string Name {
            get { return "wif"; }
        }

        public IFacetDecorator[] CreateDecorators(INakedObjectReflector reflector) {
            throw new NotImplementedException();
        }

        public IFacetDecorator[] CreateDecorators() {
            IAuthorizationManager authManager = new WifAuthorizationManager();
            //authManager.Init();
            return new IFacetDecorator[] {new SecurityFacetDecorator(authManager)};
        }

        #endregion
    }

}