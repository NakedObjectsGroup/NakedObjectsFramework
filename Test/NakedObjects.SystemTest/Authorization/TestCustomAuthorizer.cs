// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using NakedObjects.Services;
using System.Data.Entity;

namespace NakedObjects.SystemTest.Authorization.Installer  {

       public abstract class TestCustomAuthorizer : AbstractSystemTest<CustomAuthorizerInstallerDbContext> {

           protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer1 : TestCustomAuthorizer{
       
                #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthorizer1());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer1());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AttemptToUseAuthorizerForAbstractType() {
            try {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e) {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer2
    public class TestCustomAuthorizer2 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthorizer2());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer2());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AttemptToUseNonImplementationOfITestAuthorizer()
        {
            try
            {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e)
            {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer3 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthorizer3());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer3());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AttemptToUseITestAuthorizerOfObject()
        {
            try
            {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e)
            {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser4 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthoriser4());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser4());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            SetUser("Fred");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AccessByAuthorizedUserName()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser5 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthoriser5());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            SetUser("Anon");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithoutRole()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser6 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthoriser6());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser6());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            SetUser("Anon", "sysAdmin");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithRole()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass, Ignore] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser7 : TestCustomAuthorizer
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestCustomAuthoriser5());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            SetUser("Anon", "service", "sysAdmin");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithMultipleRoles()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }
    
    #region Classes used by tests
    public class CustomAuthorizerInstallerDbContext : DbContext
    {
        public const string DatabaseName = "TestCustomAuthorizerInstaller";
        public CustomAuthorizerInstallerDbContext() : base(DatabaseName) { }

        public DbSet<Foo> Foos { get; set; }
    }

    public class DefaultAuthorizer1 : ITypeAuthorizer<object> {
        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class DefaultAuthorizer2 : ITypeAuthorizer<object>
    {


        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            //Does nothing
        }
    }

    public class DefaultAuthorizer3 : ITypeAuthorizer<object>
    {
        #region ITypeAuthorizer<object> Members

        public void Init()
        {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return principal.Identity.Name == "Fred" || principal.IsInRole("sysAdmin");
        }

        public void Shutdown()
        {
            //Does nothing
        }

        #endregion
    }

    public class FooAbstractAuthorizer : ITypeAuthorizer<BarAbstract> {
        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, BarAbstract target, string memberName) {
            throw new NotImplementedException();
        }

        public bool IsVisible(IPrincipal principal, BarAbstract target, string memberName) {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public abstract class BarAbstract {
        public void Act1() {}
    }

    public class Foo {

        public virtual string Prop1 { get; set; }
    }
  #endregion
}