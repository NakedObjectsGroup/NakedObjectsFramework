// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.DatabaseHelpers;

namespace NakedObjects.Batch {
    [TestClass]
    public class BatchTest {

        protected const string Server = @"Saturn\SqlExpress";
        protected const string Database = "AdventureWorks";
        protected const string Backup = "AdventureWorks";


        [ClassInitialize]
        public static void FixtureInitialize(TestContext context) {
            var type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);

            try {
                DatabaseUtils.RestoreDatabase(Database, Backup, Server);
            }
            catch (Exception e) {
                // just carry on - tests may fail
                var m = e.Message;
                Console.WriteLine(m);
            }
        }


        [TestInitialize]
        public void Start() {
            UnityActivator.Start();
        }

        [TestCleanup]
        public void End() {
            UnityActivator.Shutdown();
        }

        [TestMethod]
        public void TestBatch() {
            var resolve = UnityConfig.GetConfiguredContainer().Resolve(typeof (IBatchRunner), null) as IBatchRunner;
            resolve.Run(new BatchStartPoint());
        }
    }
}