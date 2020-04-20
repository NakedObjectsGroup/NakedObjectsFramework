﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Core;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;
using NakedObjects.Services;
using NUnit.Framework;
//using Assert = NakedObjects.Core.Util.Assert;
using Assert = NUnit.Framework.Assert;


namespace NakedObjects.SystemTest.Authorization.Installer
{
    public abstract class TestCustomAuthorizer<TDefault> : AbstractSystemTest<CustomAuthorizerInstallerDbContext> where TDefault : ITypeAuthorizer<object>
     {

        protected override Type[] Types => new[] {typeof(TDefault), typeof(Foo) };

        protected override Type[] Services => new[] {typeof(SimpleRepository<Foo>)};

        protected override string[] Namespaces => Types.Select(t => t.Namespace).ToArray();

        protected override void RegisterTypes(IServiceCollection services) {

            base.RegisterTypes(services);
            var config = new AuthorizationConfiguration<TDefault>();

            services.AddSingleton<IAuthorizationConfiguration>(config);
            services.AddSingleton<IFacetDecorator, AuthorizationManager>();
        }
    }

    [TestFixture] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer1 : TestCustomAuthorizer<DefaultAuthorizer1>
    {
      

        [Test]
        public void AttemptToUseAuthorizerForAbstractType()
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

        #region Setup/Teardown

        [OneTimeSetUp]
        public  void ClassInitialize()
        {
            CustomAuthorizerInstallerDbContext.Delete();
            var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

            context.Database.Create();
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer2
    public class TestCustomAuthorizer2 : TestCustomAuthorizer<DefaultAuthorizer2>
    {
      

        [Test]
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

        #region Setup/Teardown

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizerInstallerDbContext.Delete();
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer3 : TestCustomAuthorizer<DefaultAuthorizer1>
    {
      

        [Test]
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

        #region Setup/Teardown

        [OneTimeSetUp]
        public  void ClassSetUp()
        {
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizerInstallerDbContext.Delete();
        }

        [SetUp]
        public void SetUp()
        {
            StartTest();
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser4 : TestCustomAuthorizer<DefaultAuthorizer3>
    {
       

        [Test]
        public void AccessByAuthorizedUserName()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }

        #region Setup/Teardown

        [OneTimeSetUp]
        public  void ClassInitialize()
        {
            CustomAuthorizerInstallerDbContext.Delete();
            var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
        }

        [SetUp]
        public void SetUp()
        {
            StartTest();
            SetUser("Fred");
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser5 : TestCustomAuthorizer<DefaultAuthorizer3>
    {
       

        [Test]
        [Ignore("investigate")]
        public void AccessByAnonUserWithoutRole()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
        }

        #region Setup/Teardown

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizerInstallerDbContext.Delete();
        }

        [SetUp]
        public void SetUp()
        {
            StartTest();
            SetUser("Anon");
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser6 : TestCustomAuthorizer<DefaultAuthorizer3>
    {
       

        [Test]
        [Ignore("investigate")]
        public void AccessByAnonUserWithRole()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }

        #region Setup/Teardown

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizerInstallerDbContext.Delete();
        }

        [SetUp]
        public void SetUp()
        {
            StartTest();
            SetUser("Anon", "sysAdmin");
        }

        #endregion
    }

    [TestFixture] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser7 : TestCustomAuthorizer<DefaultAuthorizer3>
    {
        

        [Test]
        [Ignore("investigate")]
        public void AccessByAnonUserWithMultipleRoles()
        {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }

        #region Setup/Teardown

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizerInstallerDbContext.Delete();
        }

        [SetUp]
        public void SetUp()
        {
            StartTest();
            SetUser("Anon", "service", "sysAdmin");
        }

        #endregion
    }

    #region Classes used by tests

    public class CustomAuthorizerInstallerDbContext : DbContext
    {
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

        public static void Delete() => System.Data.Entity.Database.Delete(Cs);

        public const string DatabaseName = "TestCustomAuthorizerInstaller";
        public CustomAuthorizerInstallerDbContext() : base(Cs) { }

        public DbSet<Foo> Foos { get; set; }
    }

    public class DefaultAuthorizer1 : ITypeAuthorizer<object>
    {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        #endregion

        public void Init()
        {
            //Does nothing
        }

        public void Shutdown()
        {
            //Does nothing
        }
    }

    public class DefaultAuthorizer2 : ITypeAuthorizer<object>
    {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        #endregion

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            //Does nothing
        }
    }

    public class DefaultAuthorizer3 : ITypeAuthorizer<object>
    {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return principal.Identity.Name == "Fred" || principal.IsInRole("sysAdmin");
        }

        #endregion

        public void Init()
        {
            //Does nothing
        }

        public void Shutdown()
        {
            //Does nothing
        }
    }

    public class FooAbstractAuthorizer : ITypeAuthorizer<BarAbstract>
    {
        #region ITypeAuthorizer<BarAbstract> Members

        public bool IsEditable(IPrincipal principal, BarAbstract target, string memberName)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(IPrincipal principal, BarAbstract target, string memberName)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Init()
        {
            //Does nothing
        }

        public void Shutdown()
        {
            //Does nothing
        }
    }

    public abstract class BarAbstract
    {
        public void Act1() { }
    }

    public class Foo
    {
        public virtual int Id { get; set; }
        public virtual string Prop1 { get; set; }
    }

    #endregion
}