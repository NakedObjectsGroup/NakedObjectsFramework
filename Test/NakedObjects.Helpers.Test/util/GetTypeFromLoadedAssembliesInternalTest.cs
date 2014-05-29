// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Util;

namespace NakedObjects {
    [TestClass]
    public class GetTypeFromLoadedAssembliesInternalTest : GetTypeFromLoadedAssembliesTestAbstract {

        [ClassInitialize]
        public static void Init(TestContext context) {
            SetupTypeData(context);
        }

        [ClassCleanup]
        public static void Output() {
            //OutputCsv("GetTypeFromLoadedAssembliesInternalTest");
        }

        [TestInitialize]
        public  void ClearCache() {
            TypeUtils.ClearCache();
        }

        #region test instances original implementation

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesOnce() {
            TestHarnessFindTypeFromLoadedAssembliesOnce(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesOnceRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesOnceRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallel() {
            TestHarnessFindTypeFromLoadedAssembliesInParallel(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [TestMethod]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }


        #endregion

    }
}