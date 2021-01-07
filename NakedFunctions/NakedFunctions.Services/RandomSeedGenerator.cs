// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions.Services {
    public class RandomSeedGenerator : IRandomSeedGenerator {
        public RandomSeedGenerator() {
            var now = DateTime.Now.ToFileTime();
            Random = new RandomNumber((uint) (now >> 16), (uint) (now % 4294967296));
        }

        public RandomSeedGenerator(uint u, uint v) => Random = new RandomNumber(u, v);

        public IRandom Random { get; init; }
    }
}