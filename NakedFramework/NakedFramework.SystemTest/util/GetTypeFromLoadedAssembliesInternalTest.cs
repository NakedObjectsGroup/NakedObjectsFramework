// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NUnit.Framework;

namespace NakedObjects.SystemTest.Util {
    [TestFixture]
    public class GetTypeFromLoadedAssembliesInternalTest : GetTypeFromLoadedAssembliesTestAbstract {
        [SetUp]
        public void ClearCache() {
            NakedFramework.TypeUtils.ClearCache();
        }

        [OneTimeSetUp]
        public void Init() {
            SetupTypeData();
        }

        [OneTimeTearDown]
        public void Output() {
            //OutputCsv("GetTypeFromLoadedAssembliesInternalTest");
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallel() {
            TestHarnessFindTypeFromLoadedAssembliesInParallel(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrder(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelection(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrder(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelection(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesInParallelTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesInParallelTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnce() {
            TestHarnessFindTypeFromLoadedAssembliesOnce(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnceRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomOrder(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesOnceRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesOnceRandomSelection(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimes() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimes(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomOrder() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomOrder(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }

        [Test]
        public void TestFindTypeFromLoadedAssembliesTenTimesRandomSelection() {
            TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomSelection(NakedFramework.TypeUtils.GetTypeFromLoadedAssembliesInternal);
        }
    }
}