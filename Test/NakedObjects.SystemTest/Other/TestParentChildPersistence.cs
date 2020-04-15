// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace NakedObjects.SystemTest.ParentChild
{
    namespace ParentChild
    {
        [TestFixture]
        public class TestParentChildPersistence : AbstractSystemTest<ParentChildDbContext>
        {
            protected override string[] Namespaces
            {
                get { return new[] { typeof(Parent).Namespace }; }
            }

            protected override object[] MenuServices
            {
                get
                {
                    return new object[] {
                        new SimpleRepository<Parent>(),
                        new SimpleRepository<Parent2>(),
                        new SimpleRepository<Child>()
                    };
                }
            }

            [Test]
            public virtual void CannotSaveParentIfChildHasMandatoryFieldsMissing()
            {
                var parent = GetTestService("Parents").GetAction("New Instance").InvokeReturnObject();
                var parent0 = parent.GetPropertyByName("Prop0").AssertIsMandatory().AssertIsEmpty();
                parent.AssertCannotBeSaved();
                parent0.SetValue("Foo");
                parent.AssertCannotBeSaved();

                var childProp = parent.GetPropertyByName("Child");
                var child = childProp.ContentAsObject;
                child.AssertIsType(typeof(Child));

                var child0 = child.GetPropertyByName("Prop0").AssertIsMandatory().AssertIsEmpty();
                child.AssertCannotBeSaved();
                child0.SetValue("Bar");
                child.AssertCanBeSaved();

                parent.AssertCanBeSaved();
                parent.Save();
            }

            #region Setup/Teardown

            [OneTimeSetUp]
            public  void ClassInitialize( )
            {
                InitializeNakedObjectsFramework(this);
            }

            [OneTimeTearDown]
            public  void ClassCleanup()
            {
                CleanupNakedObjectsFramework(this);
                ParentChildDbContext.Delete();
            }

            [SetUp()]
            public void SetUp()
            {
                StartTest();
            }

            #endregion
        }

        #region Classes used in tests

        public class ParentChildDbContext : DbContext
        {
            public static void Delete() => System.Data.Entity.Database.Delete(Cs);

            private static string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";


            public const string DatabaseName = "TestParentChild";
            public ParentChildDbContext() : base(Cs) { }

            public DbSet<Parent> Parents { get; set; }
            public DbSet<Parent2> Parent2s { get; set; }
            public DbSet<Child> Children { get; set; }
        }

        public class Parent
        {
            public IDomainObjectContainer Container { set; protected get; }

            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public virtual string Prop0 { get; set; }

            public virtual Child Child { get; set; }

            public void Created()
            {
                Child = Container.NewTransientInstance<Child>();
            }
        }

        public class Parent2
        {
            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public virtual string Prop0 { get; set; }

            #region Children (collection)

            private ICollection<Child> myChildren = new List<Child>();

            public virtual ICollection<Child> Children
            {
                get { return myChildren; }
                set { myChildren = value; }
            }

            public virtual void AddToChildren(Child value)
            {
                if (!(myChildren.Contains(value)))
                {
                    myChildren.Add(value);
                }
            }

            public virtual void RemoveFromChildren(Child value)
            {
                if (myChildren.Contains(value))
                {
                    myChildren.Remove(value);
                }
            }

            #endregion
        }

        public class Child
        {
            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public virtual string Prop0 { get; set; }
        }

        #endregion
    }
}