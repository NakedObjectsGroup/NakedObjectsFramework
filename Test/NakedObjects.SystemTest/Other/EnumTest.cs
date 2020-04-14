// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ITestAction = NakedObjects.Xat.ITestAction;
using TestContext = NUnit.Framework.TestContext;

namespace NakedObjects.SystemTest.Enum
{
    [TestFixture]
    public class EnumTest : AbstractSystemTest<EnumDbContext>
    {
        protected override Type[] Types
        {
            get { return new[] { typeof(Foo), typeof(Sexes), typeof(HairColours) }; }
        }

        #region Run configuration

        protected override object[] MenuServices
        {
            get { return (new object[] { new SimpleRepository<Foo>() }); }
        }

        #endregion

        [Test]
        public virtual void EnumPropertyBasic()
        {
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

        [Test]
        public virtual void EnumPropertyWithDefault()
        {
            ITestObject foo = NewTestObject<Foo>();
            //Property with no default
            foo.GetPropertyByName("Sex1").AssertValueIsEqual("Male");
            //Property with default
            foo.GetPropertyByName("Sex2").AssertValueIsEqual("Unknown");
        }

        [Test]
        public virtual void EnumPropertyWithChoices()
        {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex3").AssertValueIsEqual("Male");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(2, values.Count());
            Assert.AreEqual("Male", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Female", values.ElementAt(1).NakedObject.TitleString());
        }

        [Test]
        public virtual void EnumPropertyWithChoicesAndDefault()
        {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex4").AssertValueIsEqual("Male");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(2, values.Count());
        }

        [Test]
        public virtual void EnumPropertyByteEnum()
        {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Hair Colour1");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(5, values.Count());
            Assert.AreEqual("Black", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("White", values.ElementAt(4).NakedObject.TitleString());

            sex1.AssertFieldEntryIsValid("Brunette");
            sex1.AssertFieldEntryInvalid("Fair");
        }

        [Test]
        public virtual void IntPropertyAsEnum()
        {
            ITestObject foo = NewTestObject<Foo>();

            ITestProperty sex1 = foo.GetPropertyByName("Sex5");
            ITestNaked[] values = sex1.GetChoices();
            Assert.AreEqual(4, values.Count());
            Assert.AreEqual("Female", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Male", values.ElementAt(1).NakedObject.TitleString());
            Assert.AreEqual("Not Specified", values.ElementAt(2).NakedObject.TitleString());
            Assert.AreEqual("Unknown", values.ElementAt(3).NakedObject.TitleString());
        }

        [Test]
        public virtual void EnumParameter()
        {
            ITestObject foo = NewTestObject<Foo>();
            ITestAction act1 = foo.GetAction("Action1");
            ITestNaked[] values = act1.Parameters[0].GetChoices();
            Assert.AreEqual(4, values.Count());
            Assert.AreEqual("Female", values.ElementAt(0).NakedObject.TitleString());
            Assert.AreEqual("Male", values.ElementAt(1).NakedObject.TitleString());
            Assert.AreEqual("Not Specified", values.ElementAt(2).NakedObject.TitleString());
            Assert.AreEqual("Unknown", values.ElementAt(3).NakedObject.TitleString());
        }

        [Test]
        public virtual void EnumParameterWithDefault()
        {
            ITestObject foo = NewTestObject<Foo>();
            ITestAction act1 = foo.GetAction("Action1");
            Assert.AreEqual(null, act1.Parameters[0].GetDefault());

            ITestAction act2 = foo.GetAction("Action2");
            Assert.AreEqual("Unknown", act2.Parameters[0].GetDefault().Title);
        }

        #region Setup/Teardown

        [OneTimeSetUp]
        public  void ClassInitialize( )
        {
            EnumDbContext.Delete();
            var context = Activator.CreateInstance<EnumDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
        }

        [SetUp()]
        public void SetUp()
        {
            StartTest();
        }

        [TearDown()]
        public void TearDown() { }

        #endregion
    }

    #region Classes used in tests

    public class EnumDbContext : DbContext
    {
        public static void Delete() => System.Data.Entity.Database.Delete(Cs);

        private static string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

        public const string DatabaseName = "TestEnums";
        public EnumDbContext() : base(Cs) { }

        public DbSet<Foo> Foos { get; set; }
    }

    public class Foo
    {
        public virtual int Id { get; set; }

        #region Sex1

        public virtual Sexes Sex1 { get; set; }

        #endregion

        #region Sex5

        [EnumDataType(typeof(Sexes))]
        public virtual int Sex5 { get; set; }

        #endregion

        #region HairColour1

        public virtual HairColours HairColour1 { get; set; }

        #endregion

        #region Action1

        public void Action1(Sexes sex) { }

        #endregion

        #region Sex2

        public virtual Sexes Sex2 { get; set; }

        public Sexes DefaultSex2()
        {
            return Sexes.Unknown;
        }

        #endregion

        #region Sex3

        public virtual Sexes Sex3 { get; set; }

        public Sexes[] ChoicesSex3()
        {
            return new[] { Sexes.Male, Sexes.Female };
        }

        #endregion

        #region Sex4

        public virtual Sexes Sex4 { get; set; }

        public Sexes[] ChoicesSex4()
        {
            return new[] { Sexes.Male, Sexes.Female };
        }

        public Sexes DefaultSex4()
        {
            return Sexes.Male;
        }

        #endregion

        #region Action2

        public void Action2(Sexes sex) { }

        public Sexes Default0Action2()
        {
            return Sexes.Unknown;
        }

        #endregion
    }

    public enum Sexes
    {
        Male = 1,
        Female = 2,
        Unknown = 3,
        NotSpecified = 4
    }

    public enum HairColours : byte
    {
        Black = 1,
        Blond = 2,
        Brunette = 3,
        Grey = 4,
        White = 5
    }

    #endregion
}