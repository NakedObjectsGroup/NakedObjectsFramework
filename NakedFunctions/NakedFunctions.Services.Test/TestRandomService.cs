// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedFunctions.Services.Test {
    [TestClass]
    public class TestRandomService {
        [TestMethod]
        public void Random1() {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            var random = seeder.Random;
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++) {
                sb.Append(random.ValueInRange(0, 10)).Append(" ");
                random = random.Next();
            }

            var actual = sb.ToString();
            Assert.AreEqual("4 0 1 6 5 0 9 8 2 7 ", actual);
        }

        [TestMethod]
        public void Random2() {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(362436069, 521288629);
            var random = seeder.Random;
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++) {
                sb.Append(random.ValueInRange(0, 10)).Append(" ");
                random = random.Next();
            }

            var actual = sb.ToString();
            Assert.AreEqual("6 3 8 8 5 2 4 1 3 8 ", actual);
        }

        [TestMethod]
        public void Random3() {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            var random = seeder.Random;
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++) {
                sb.Append(random.ValueInRange(3, 5)).Append(" ");
                random = random.Next();
            }

            var actual = sb.ToString();
            Assert.AreEqual("3 3 4 3 4 3 4 3 3 4 ", actual);
        }

        [TestMethod]
        public void Random4() {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            var random = seeder.Random;
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++) {
                sb.Append(random.ValueInRange(4)).Append(" ");
                random = random.Next();
            }

            var actual = sb.ToString();
            Assert.AreEqual("2 0 3 0 3 0 1 2 2 1 ", actual);
        }

        [TestMethod] //Very crude test to check that there is an approximately even distribution of 0, 1 results
        public void RandomSeedFromClock() {
            IRandomSeedGenerator seeder = new RandomSeedGenerator();
            var random = seeder.Random;
            var zeros = 0;
            var ones = 0;
            for (var i = 0; i < 1000; i++) {
                random = random.Next();
                if (random.ValueInRange(0, 2) == 0) {
                    zeros += 1;
                }
                else {
                    ones += 1;
                }
            }

            Assert.AreEqual(1000, zeros + ones);
            Assert.IsTrue(zeros < 550);
            Assert.IsTrue(ones < 550);
        }
    }
}