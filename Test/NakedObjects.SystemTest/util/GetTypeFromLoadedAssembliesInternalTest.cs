// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using NakedObjects.Util;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Util {
    [TestFixture]
    public class GetTypeFromLoadedAssembliesInternalTest : GetTypeFromLoadedAssembliesTestAbstract {
        [OneTimeSetUp]
        public void Init() {
            SetupTypeData();
        }

        [OneTimeTearDown]
        public void Output() {
            //OutputCsv("GetTypeFromLoadedAssembliesInternalTest");
        }

        [SetUp]
        public void ClearCache() {
            TypeUtils.ClearCache();
        }

        #region test instances original implementation

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnce() {
            TestHarnessFindTypeFromLoadedAssembliesOnce(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnceRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnceRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallel() {
            TestHarnessFindTypeFromLoadedAssembliesInParallel(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrder(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelection(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes(TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        #endregion
    }
}