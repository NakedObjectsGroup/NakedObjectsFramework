// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;

namespace NakedObjects.SystemTest.Attributes
{
    namespace Range
    {
        [TestClass]
        public class TestRangeAttribute : AbstractSystemTest
        {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest()
            {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
            }

            [TestCleanup()]
            public void TearDownTest()
            {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures
            {
                get { return new FixturesInstaller(new object[] { }); }
            }

            protected override IServicesInstaller MenuServices
            {
                get { return new ServicesInstaller(new object[] { new SimpleRepository<Object1>() }); }
            }
            #endregion

            private static string todayMinus31 = DateTime.Today.AddDays(-31).ToShortDateString();
            private static string todayMinus30 = DateTime.Today.AddDays(-30).ToShortDateString();
            private static string todayMinus1 = DateTime.Today.AddDays(-1).ToShortDateString();
            private static string today = DateTime.Today.ToShortDateString();
            private static string todayPlus1 = DateTime.Today.AddDays(1).ToShortDateString();
            private static string todayPlus30 = DateTime.Today.AddDays(30).ToShortDateString();
            private static string todayPlus31 = DateTime.Today.AddDays(31).ToShortDateString();

            [TestMethod]
            public virtual void RangeOnNumericProperties()
            {
                NumericPropertyRangeTest("Prop2");
                NumericPropertyRangeTest("Prop3");
                NumericPropertyRangeTest("Prop4");
                NumericPropertyRangeTest("Prop5");
                NumericPropertyRangeTest("Prop6");
                NumericPropertyRangeTest("Prop7");
                NumericPropertyRangeTest("Prop8");
                NumericPropertyRangeTest("Prop9");
                NumericPropertyRangeTest("Prop10");
                NumericPropertyRangeTest("Prop11");
                NumericPropertyRangeTest("Prop12");
                NumericPropertyRangeTest("Prop13");
                NumericPropertyRangeTest("Prop14");
                NumericPropertyRangeTest("Prop15");
                NumericPropertyRangeTest("Prop16");
                NumericPropertyRangeTest("Prop17");
                NumericPropertyRangeTest("Prop18");
                NumericPropertyRangeTest("Prop19");
                NumericPropertyRangeTest("Prop20");
                NumericPropertyRangeTest("Prop21");
                NumericPropertyRangeTest("Prop22");
                NumericPropertyRangeTest("Prop23");
            }


            private void NumericPropertyRangeTest(string name)
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop = obj.GetPropertyById(name);
                try
                {
                    prop.AssertFieldEntryInvalid("-2");
                    prop.AssertFieldEntryInvalid("11");
                    prop.SetValue("1");
                }
                catch
                {
                    Console.WriteLine("Failed " + name);
                    throw;
                }
            }

            [TestMethod]
            public virtual void RangeOnDateProperties()
            {



                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop = obj.GetPropertyById("Prop25");
                try
                {
                    prop.AssertFieldEntryInvalid(todayMinus31);
                    prop.AssertFieldEntryIsValid(todayMinus30);
                    prop.AssertFieldEntryIsValid(today);
                    prop.AssertFieldEntryInvalid(todayPlus1);
                    prop.SetValue(todayMinus1);
                }
                catch
                {
                    Console.WriteLine("Failed " + "Prop25");
                    throw;
                }

                
                prop = obj.GetPropertyById("Prop26");
                try
                {
                    prop.AssertFieldEntryInvalid(today);
                    prop.AssertFieldEntryIsValid(todayPlus1);
                    prop.AssertFieldEntryIsValid(todayPlus30);
                    prop.AssertFieldEntryInvalid(todayPlus31);
                    prop.SetValue(todayPlus1);
                }
                catch
                {
                    Console.WriteLine("Failed " + "Prop25");
                    throw;
                }
            }


            [TestMethod]  
            public virtual void RangeOnDateParms()
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestAction act = obj.GetAction("Action 24");
                try
                {
                     act.AssertIsInvalidWithParms(todayMinus31);
                    act.AssertIsValidWithParms(todayMinus30);
                    act.AssertIsValidWithParms(today);
                    act.AssertIsInvalidWithParms(todayPlus1);
                    act.InvokeReturnObject(todayMinus1);
                }
                catch
                {
                    Console.WriteLine("Failed " + "Action24");
                    throw;
                }

                 act = obj.GetAction("Action 25");
                try
                {
                    act.AssertIsInvalidWithParms(today);
                    act.AssertIsValidWithParms(todayPlus1);
                    act.AssertIsValidWithParms(todayPlus30);
                    act.AssertIsInvalidWithParms(todayPlus31);
                    act.InvokeReturnObject(todayPlus1);
                }
                catch
                {
                    Console.WriteLine("Failed " + "Action25");
                    throw;
                }
            }

            private void NumericParmRangeTest(string name)
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestAction act = obj.GetAction(name);

                try
                {
                    act.AssertIsInvalidWithParms(4);
                    act.AssertIsValidWithParms(5);
                    act.AssertIsValidWithParms(6);
                    act.AssertIsInvalidWithParms(7);
                }
                catch
                {
                    Console.WriteLine("Failed " + name);
                    throw;
                }
            }

            [TestMethod]
            public virtual void RangeOnNumericParms()
            {
                NumericParmRangeTest("Action1");
                NumericParmRangeTest("Action2");
                NumericParmRangeTest("Action3");
                NumericParmRangeTest("Action4");
                NumericParmRangeTest("Action5");
                NumericParmRangeTest("Action6");
                NumericParmRangeTest("Action7");
                NumericParmRangeTest("Action8");
                NumericParmRangeTest("Action9");
                NumericParmRangeTest("Action 10");
                NumericParmRangeTest("Action 11");
                NumericParmRangeTest("Action 12");
                NumericParmRangeTest("Action 13");
                NumericParmRangeTest("Action 14");
                NumericParmRangeTest("Action 15");
                NumericParmRangeTest("Action 16");
                NumericParmRangeTest("Action 17");
                NumericParmRangeTest("Action 18");
                NumericParmRangeTest("Action 19");
                NumericParmRangeTest("Action 20");
                NumericParmRangeTest("Action 21");
                NumericParmRangeTest("Action 22");
            }

            [TestMethod]
            public virtual void UnsupportedRangeOnProperty()
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop = obj.GetPropertyById("Prop24");
                prop.SetValue("-2");
                prop.SetValue("11");
                prop.SetValue("1");
            }

            [TestMethod]
            public virtual void UnsupportedRangeOnParm()
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestAction act = obj.GetAction("Action 23");
                act.AssertIsValidWithParms(4);
                act.AssertIsValidWithParms(5);
                act.AssertIsValidWithParms(10);
            }

        }

        public struct Comparable : System.IComparable
        {
            #region IComparable Members

            public int CompareTo(object obj)
            {
                return -1;
            }

            #endregion
        }

        public class Object1
        {
            public string Prop1 { get; set; }

            [Range(-1, 10)]
            public sbyte Prop2 { get; set; }

            [Range(-1, 10)]
            public short Prop3 { get; set; }

            [Range(-1, 10)]
            public int Prop4 { get; set; }

            [Range(-1, 10)]
            public long Prop5 { get; set; }

            [Range(1, 10)]
            public byte Prop6 { get; set; }

            [Range(1, 10)]
            public ushort Prop7 { get; set; }

            [Range(1, 10)]
            public uint Prop8 { get; set; }

            [Range(1, 10)]
            public ulong Prop9 { get; set; }

            [Range(-1, 10)]
            public float Prop10 { get; set; }

            [Range(-1, 10)]
            public double Prop11 { get; set; }

            [Range(-1, 10)]
            public decimal Prop12 { get; set; }


            [Range(-1d, 10d)]
            public sbyte Prop13 { get; set; }

            [Range(-1d, 10d)]
            public short Prop14 { get; set; }

            [Range(-1d, 10d)]
            public int Prop15 { get; set; }

            [Range(-1d, 10d)]
            public long Prop16 { get; set; }

            [Range(1d, 10d)]
            public byte Prop17 { get; set; }

            [Range(1d, 10d)]
            public ushort Prop18 { get; set; }

            [Range(1d, 10d)]
            public uint Prop19 { get; set; }

            [Range(1d, 10d)]
            public ulong Prop20 { get; set; }

            [Range(-1.9d, 10.9d)]
            public float Prop21 { get; set; }

            [Range(-1.9d, 10.9d)]
            public double Prop22 { get; set; }

            [Range(-1.9d, 10.9d)]
            public decimal Prop23 { get; set; }

            [Range(typeof(string), "1", "10")]
            public int Prop24 { get; set; }

            [Range(-30, 0)]
            public DateTime Prop25 { get; set; }

            [Range(1, 30)]
            public DateTime Prop26 { get; set; }

            public void Action1([Range(5, 6)]sbyte parm) { }
            public void Action2([Range(5, 6)]short parm) { }
            public void Action3([Range(5, 6)]int parm) { }
            public void Action4([Range(5, 6)]long parm) { }
            public void Action5([Range(5, 6)]byte parm) { }
            public void Action6([Range(5, 6)]ushort parm) { }
            public void Action7([Range(5, 6)]uint parm) { }
            public void Action8([Range(5, 6)]ulong parm) { }
            public void Action9([Range(5, 6)]float parm) { }
            public void Action10([Range(5, 6)]double parm) { }
            public void Action11([Range(5, 6)]decimal parm) { }
            public void Action12([Range(5d, 6d)]sbyte parm) { }
            public void Action13([Range(5d, 6d)]short parm) { }
            public void Action14([Range(5d, 6d)]int parm) { }
            public void Action15([Range(5d, 6d)]long parm) { }
            public void Action16([Range(5d, 6d)]byte parm) { }
            public void Action17([Range(5d, 6d)]ushort parm) { }
            public void Action18([Range(5d, 6d)]uint parm) { }
            public void Action19([Range(5d, 6d)]ulong parm) { }
            public void Action20([Range(5d, 6d)]float parm) { }
            public void Action21([Range(5d, 6d)]double parm) { }
            public void Action22([Range(5d, 6d)]decimal parm) { }
            public void Action23([Range(typeof(string), "5", "6")]int parm) { }

            public void Action24([Range(-30, 0)] DateTime parm) { }
            public void Action25([Range(1, 30)] DateTime parm) { }

        }
    }
} //end of root namespace