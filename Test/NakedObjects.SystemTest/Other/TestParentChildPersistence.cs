// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NakedObjects.SystemTest.ParentChild {
    namespace ParentChild {
        [TestClass]
        public class TestParentChildPersistence : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
            }

            #endregion

            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Parent>(),
                                                                  new SimpleRepository<Parent2>(), 
                                                                  new SimpleRepository<Child>()
                                                              });
                }
            }

            [TestMethod]
            public virtual void CannotSaveParentIfChildHasMandatoryFieldsMissing() {

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
                child.Save();

                parent.AssertCanBeSaved();
                parent.Save();
            }

            [TestMethod, Ignore] // now that collectiona are disable dby default 
            public virtual void ParentWithCollectionOfChildren()
            {
                var parent = NewTestObject<Parent2>();
                parent.AssertCannotBeSaved();
                parent.GetPropertyByName("Prop0").AssertIsMandatory().AssertIsEmpty().SetValue("Joe");
                parent.AssertCanBeSaved();
                
                var child1 = NewTestObject<Child>();
                var child2 = NewTestObject<Child>();

                var children = parent.GetPropertyByName("Children");
                children.SetObject(child1);
                children.SetObject(child2);

                children.ContentAsCollection.AssertCountIs(2);

                parent.AssertCannotBeSaved();

                child1.GetPropertyByName("Prop0").SetValue("Bar");
                child1.Save();

                parent.AssertCannotBeSaved();

                child2.GetPropertyByName("Prop0").SetValue("Foo");
                child2.Save();

                parent.Save();
            }
        }

        public class Parent {

            public Parent()
            {
                Child = new Child();
            }

            public string Prop0 { get; set; }

            public Child Child { get; set; }
           
        }

        public class Parent2
        {


            public string Prop0 { get; set; }

            #region Children (collection)
            private ICollection<Child> myChildren = new List<Child>();

            public virtual ICollection<Child> Children
            {
                get
                {
                    return myChildren;
                }
                set
                {
                    myChildren = value;
                }
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
            public string Prop0 { get; set; }

        }

    }
} //end of root namespace