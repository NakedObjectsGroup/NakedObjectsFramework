// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Threading;

namespace NakedFramework.Xat.TestCase {
    public class FixtureServices {
        public FixtureServices() => Clock = FixtureClock.Initialize();

        protected FixtureClock Clock { get; }

        public void ResetClock() {
            Clock.Reset();
        }

        #region Nested type: FixtureClock

        protected class FixtureClock {
            private DateTime time;

            /// <summary>
            ///     Create a return new FixtureClock
            /// </summary>
            public static FixtureClock Initialize() => new();

            /// <summary>
            ///     SetupContexts time to now
            /// </summary>
            public void Reset() {
                time = DateTime.Now;
            }

            public override string ToString() => time.ToString(Thread.CurrentThread.CurrentCulture);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}