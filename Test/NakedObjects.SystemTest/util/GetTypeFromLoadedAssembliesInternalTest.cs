// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Util;

namespace NakedObjects.SystemTest.Util {
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
        public void ClearCache() {
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