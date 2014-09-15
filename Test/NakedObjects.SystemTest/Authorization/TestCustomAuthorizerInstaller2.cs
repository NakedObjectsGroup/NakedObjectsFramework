// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using NakedObjects.Services;

namespace NakedObjects.SystemTest.Authorization.Installer2 {
    [TestClass, Ignore]
    public class TestCustomAuthorizerInstaller2 : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        //protected override IAuthorizerInstaller Authorizer {
        //    get {
                
        //        return new CustomAuthorizerInstaller( new MyDefaultAuthorizer());
        //    }
        //}

        #endregion

        [TestMethod]
        public void AttemptToUseNonImplementationOfITestAuthorizer() {
            try {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e) {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
     

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public void Init() {
            throw new System.NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class Foo {
        [Optionally]
        public virtual string Prop1 { get; set; }
    }
}