// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Validate {
        [TestClass]
        public class TestValidationMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
                //Set up any common variables here
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                //Tear down any common variables here
            }

            #endregion

            #region "Services & Fixtures"
            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>(),
                                                                   new SimpleRepository<Object5>()
                                                              });
                }
            }
            #endregion

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            [TestMethod]
            public void UnmatchedValidateMethodShowsUpAsAnAction() {
                ITestObject obj = NewTestObject<Object3>();
                obj.GetAction("Validate Prop1");
                obj.GetAction("Validate Prop2");
                obj.GetAction("Validate Prop3");
                obj.GetAction("Validate Do Something");
                obj.GetAction("Validate 0 Do Something");
                obj.GetAction("Validate 1 Do Something");
            }

            [TestMethod]
            public void ValidateMethodDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Validate Prop1");
                    Assert.Fail("'Validate Prop1' is showing as an action");
                }
                catch (AssertFailedException e) {
                    Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Prop1'", e.Message);
                }

                try {
                    obj.GetAction("Validate0 Do Something");
                    Assert.Fail("'Validate0 Do Something' is showing as an action");
                }
                catch (AssertFailedException e)
                {
                    Assert.AreEqual("Assert.Fail failed. No Action named 'Validate0 Do Something'", e.Message);
                }

                try {
                    obj.GetAction("Validate Do Something Else");
                    Assert.Fail("'Validate Do Something Else' is showing as an action");
                }
                catch (AssertFailedException e)
                {
                    Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Do Something Else'", e.Message);
                }
            }

            [TestMethod]
            public virtual void ValidateNumericalProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertFieldEntryInvalid("2").AssertLastMessageIs("Value must be between 3 & 10");
                prop1.AssertFieldEntryInvalid("11").AssertLastMessageIs("Value must be between 3 & 10");
                prop1.SetValue("6").AssertLastMessageIs("");
                try {
                    prop1.SetValue("11");
                    Assert.Fail();
                }
                catch (Exception) {
                    prop1.SetValue("7");
                }
            }

            [TestMethod]
            public virtual void ValidateStringProperty()
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop1 = obj.GetPropertyByName("Prop2");
                prop1.AssertFieldEntryInvalid("foo").AssertLastMessageIs("Value must start with a");
                prop1.AssertFieldEntryInvalid("bar").AssertLastMessageIs("Value must start with a");
                prop1.SetValue("afoo").AssertLastMessageIs("");
                try
                {
                    prop1.SetValue("bar");
                    Assert.Fail();
                }
                catch (Exception)
                {
                    prop1.SetValue("abar");
                }
            }



            [TestMethod]
            public virtual void ValidateReferenceProperty() {
                ITestObject obj1 = NewTestObject<Object1>();
                ITestProperty obj1Prop3 = obj1.GetPropertyByName("Prop3");

                ITestObject obj2a = NewTestObject<Object2>();
                obj2a.GetPropertyByName("Prop1").SetValue("a");
                ITestObject obj2b = NewTestObject<Object2>();
                obj2b.GetPropertyByName("Prop1").SetValue("b");

                obj1Prop3.AssertSetObjectInvalid(obj2b).AssertLastMessageIs("Invalid Object");
                obj1Prop3.AssertSetObjectIsValid(obj2a).AssertLastMessageIs("");

                try {
                    obj1Prop3.SetObject(obj2b);
                    Assert.Fail();
                }
                catch (Exception) {
                    obj1Prop3.SetObject(obj2a);
                }
            }

            [TestMethod]
            public void ValidateParametersIndividually()
            {
                ITestObject obj1 = NewTestObject<Object1>();
                ITestAction action = obj1.GetAction("Do Something");

                ITestObject obj2a = NewTestObject<Object2>();
                obj2a.GetPropertyByName("Prop1").SetValue("a");
                ITestObject obj2b = NewTestObject<Object2>();
                obj2b.GetPropertyByName("Prop1").SetValue("b");

                action.InvokeReturnObject(new object[] { 5, "abar", obj2a });

                action.AssertIsInvalidWithParms(new object[] { 2, "abar", obj2a }).AssertLastMessageIs("Value must be between 3 & 10");
                action.AssertIsInvalidWithParms(new object[] { 5, "bar", obj2a }).AssertLastMessageIs("Value must start with a");
                action.AssertIsInvalidWithParms(new object[] { 5, "abar", obj2b }).AssertLastMessageIs("Invalid Object");
            }

            [TestMethod]
            public void ValidateParametersCollectively()
            {
                ITestObject obj1 = NewTestObject<Object1>();
                ITestAction action = obj1.GetAction("Do Something Else");

                ITestObject obj2a = NewTestObject<Object2>();
                obj2a.GetPropertyByName("Prop1").SetValue("a");
                ITestObject obj2b = NewTestObject<Object2>();
                obj2b.GetPropertyByName("Prop1").SetValue("b");

                action.InvokeReturnObject(new object[] { 5, "abar", obj2a });

                action.AssertIsInvalidWithParms(new object[] { 2, "abar", obj2a }).AssertLastMessageIs("Something amiss");
                action.AssertIsInvalidWithParms(new object[] { 5, "bar", obj2a }).AssertLastMessageIs("Something amiss");
                action.AssertIsInvalidWithParms(new object[] { 5, "abar", obj2b }).AssertLastMessageIs("Something amiss");
            }


            [TestMethod]
            public virtual void ValidateCrossValidationFail4() {
                ITestObject obj = NewTestObject<Object4>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value2");

                try {
                    obj.Save();
                    Assert.Fail("Expect exception");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. Expect prop1 == prop2", e.Message);
                }
            }

            [TestMethod]
            public virtual void ValidateCrossValidationSuccess4() {
                ITestObject obj = NewTestObject<Object4>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value1");
                obj.Save();
            }


            [TestMethod]
            public virtual void ValidateCrossValidationFail5A() {
                ITestObject obj = NewTestObject<Object5>();
                ITestObject obj4 = NewTestObject<Object4>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value2");
                obj.GetPropertyByName("Prop3").SetValue("1");
                obj.GetPropertyByName("Prop4").SetObject(obj4);
                  
                try {
                    obj.Save();
                    Assert.Fail("Expect exception");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
                }
            }

            [TestMethod]
            public virtual void ValidateCrossValidationFail5B() {
                ITestObject obj = NewTestObject<Object5>();
                ITestObject obj4 = NewTestObject<Object4>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value1");
                obj.GetPropertyByName("Prop3").SetValue("0");
                obj.GetPropertyByName("Prop4").SetObject(obj4);


                try {
                    obj.Save();
                    Assert.Fail("Expect exception");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
                }
            }

            [TestMethod]
            public virtual void ValidateCrossValidationFail5C() {
                ITestObject obj = NewTestObject<Object5>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value1");
                obj.GetPropertyByName("Prop3").SetValue("1");
                obj.GetPropertyByName("Prop4").ClearObject();

                try {
                    obj.Save();
                    Assert.Fail("Expect exception");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
                }
            }


            [TestMethod]
            public virtual void ValidateCrossValidationSuccess5() {
                ITestObject obj = NewTestObject<Object5>();
                ITestObject obj4 = NewTestObject<Object4>();
                obj.GetPropertyByName("Prop1").SetValue("value1");
                obj.GetPropertyByName("Prop2").SetValue("value1");
                obj.GetPropertyByName("Prop3").SetValue("1");
                obj.GetPropertyByName("Prop4").SetObject(obj4);
              
                obj.Save();
            }


        }





        public class Object1 {
            public int Prop1 { get; set; }

            public string Prop2 { get; set; }

            public Object2 Prop3 { get; set; }

            public string ValidateProp1(int value) {
                if (value < 3 || value > 10) {
                    return "Value must be between 3 & 10";
                }
                return null;
            }

            public string ValidateProp2(string value) {
                if (!value.StartsWith("a")) {
                    return "Value must start with a";
                }
                return null;
            }

            public string ValidateProp3(Object2 value) {
                if (!value.Prop1.StartsWith("a")) {
                    return "Invalid Object";
                }
                return null;
            }

            #region Do Something

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public string Validate0DoSomething(int value) {
                return ValidateProp1(value);
            }

            public string Validate1DoSomething(string value) {
                return ValidateProp2(value);
            }

            public string Validate2DoSomething(Object2 value) {
                return ValidateProp3(value);
            }

            #endregion

            #region Do Something Else

            public void DoSomethingElse(int param0, string param1, Object2 param2) {}

            public string ValidateDoSomethingElse(int param0, string param1, Object2 param2) {
                if (ValidateProp1(param0) != null || ValidateProp2(param1) != null || ValidateProp3(param2) != null) {
                    return "Something amiss";
                }
                return null;
            }

            #endregion
        }

        public class Object2 {
            public string Prop1 { get; set; }
        }

        public class Object3 {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public string ValidateProp1(int value) {
                return null;
            }

            public bool ValidateProp2(string value) {
                return false;
            }

            public string ValidateProp3(string value) {
                return null;
            }

            public void DoSomething(int par1) {}

            public string ValidateDoSomething(decimal par1) {
                return null;
            }

            public string Validate0DoSomething(bool par1) {
                return null;
            }

            public string Validate1DoSomething(int par1) {
                return null;
            }
        }


        public class Object4 {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public string Validate(string prop1, string prop2) {
                if (prop1 != prop2) {
                    return "Expect prop1 == prop2";
                }
                return null;
            }
        }


        public class Object5 {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public int Prop3 { get; set; }

            [Optionally]
            public Object4 Prop4 { get; set; }

            public string Validate(Object4 prop4, string prop1, int prop3, string prop2) {
                if (prop1 != prop2 || prop3 == 0 || prop4 == null) {
                    return "Condition Fail";
                }
                return null;
            }
        }



    }
}