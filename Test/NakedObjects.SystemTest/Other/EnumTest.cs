// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Enum {
    [TestClass]
    public class EnumTest : AbstractSystemTest {
        #region Run configuration

        //Set up the properties in this region exactly the same way as in your Run class

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        //protected override IObjectPersistorInstaller Persistor
        //{
        //    get { return new EntityPersistorInstaller(); }
        //}

        #endregion

        #region Initialize and Cleanup

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework(this);
        }

        #endregion

        [TestMethod]
        public virtual void EnumPropertyBasic() {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex1");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(4, values.Count());
            Assert.AreEqual("Female", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Male", values.ElementAt(1).NakedObject.TitleString());
            Assert.AreEqual("Not Specified", values.ElementAt(2).NakedObject.TitleString());
            Assert.AreEqual("Unknown", values.ElementAt(3).NakedObject.TitleString());

            sex1.AssertFieldEntryIsValid("Male");
            sex1.AssertFieldEntryInvalid("Man");
        }

        [TestMethod]
        public virtual void EnumPropertyWithDefault() {
            ITestObject foo = NewTestObject<Foo>();
            //Property with no default
            foo.GetPropertyByName("Sex1").AssertValueIsEqual("0");
            //Property with default
            foo.GetPropertyByName("Sex2").AssertValueIsEqual("Unknown");
        }

        [TestMethod]
        public virtual void EnumPropertyWithChoices() {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex3").AssertValueIsEqual("0");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(2, values.Count());
            Assert.AreEqual("Male", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Female", values.ElementAt(1).NakedObject.TitleString());
        }

        [TestMethod]
        public virtual void EnumPropertyWithChoicesAndDefault() {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex4").AssertValueIsEqual("Male");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(2, values.Count());
        }

        [TestMethod]
        public virtual void EnumPropertyByteEnum() {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Hair Colour1");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(5, values.Count());
            Assert.AreEqual("Black", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("White", values.ElementAt(4).NakedObject.TitleString());

            sex1.AssertFieldEntryIsValid("Brunette");
            sex1.AssertFieldEntryInvalid("Fair");
        }

        [TestMethod]
        public virtual void IntPropertyAsEnum() {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex5");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(4, values.Count());
            Assert.AreEqual("Female", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Male", values.ElementAt(1).NakedObject.TitleString());
            Assert.AreEqual("Not Specified", values.ElementAt(2).NakedObject.TitleString());
            Assert.AreEqual("Unknown", values.ElementAt(3).NakedObject.TitleString());
        }

        [TestMethod]
        public virtual void EnumParameter() {
            ITestObject foo = NewTestObject<Foo>();
            ITestAction act1 = foo.GetAction("Action1");
            ITestNaked[] values = act1.Parameters[0].GetChoices();
            Assert.AreEqual(4, values.Count());
            Assert.AreEqual("Female", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Male", values.ElementAt(1).NakedObject.TitleString());
            Assert.AreEqual("Not Specified", values.ElementAt(2).NakedObject.TitleString());
            Assert.AreEqual("Unknown", values.ElementAt(3).NakedObject.TitleString());
        }

        [TestMethod]
        public virtual void EnumParameterWithDefault() {
            ITestObject foo = NewTestObject<Foo>();
            ITestAction act1 = foo.GetAction("Action1");
            Assert.AreEqual(null, act1.Parameters[0].GetDefault());

            ITestAction act2 = foo.GetAction("Action2");
            Assert.AreEqual("Unknown", act2.Parameters[0].GetDefault().Title);
        }
    }

    public class Foo {
        #region Sex1

        public virtual Sexes Sex1 { get; set; }

        #endregion

        #region Sex2

        public virtual Sexes Sex2 { get; set; }

        public Sexes DefaultSex2() {
            return Sexes.Unknown;
        }

        #endregion

        #region Sex3

        public virtual Sexes Sex3 { get; set; }


        public Sexes[] ChoicesSex3() {
            return new[] {Sexes.Male, Sexes.Female};
        }

        #endregion

        #region Sex4

        public virtual Sexes Sex4 { get; set; }

        public Sexes[] ChoicesSex4() {
            return new[] {Sexes.Male, Sexes.Female};
        }

        public Sexes DefaultSex4() {
            return Sexes.Male;
        }

        #endregion

        #region Sex5

        [EnumDataType(typeof (Sexes))]
        public virtual int Sex5 { get; set; }

        #endregion

        #region HairColour1

        public virtual HairColours HairColour1 { get; set; }

        #endregion

        #region Action1

        public void Action1(Sexes sex) {}

        #endregion

        #region Action2

        public void Action2(Sexes sex) {}

        public Sexes Default0Action2() {
            return Sexes.Unknown;
        }

        #endregion
    }

    public enum Sexes {
        Male = 1,
        Female = 2,
        Unknown = 3,
        NotSpecified = 4
    }

    public enum HairColours : byte {
        Black = 1,
        Blond = 2,
        Brunette = 3,
        Grey = 4,
        White = 5
    }
}