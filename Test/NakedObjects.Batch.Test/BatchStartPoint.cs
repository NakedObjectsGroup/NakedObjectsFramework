// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksModel;
using NakedObjects.Architecture.Component;
using NakedObjects.Async;
using NakedObjects.Core.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Batch {
    public class BatchStartPoint : IBatchStartPoint {
        private const int StartId = 63290;
        private const int CountIds = 20;
        private readonly string testMessage = Guid.NewGuid().ToString();
        public IAsyncService AsyncService { private get; set; }
        public IDomainObjectContainer Container { private get; set; }

        #region IBatchStartPoint Members

        public void Execute() {
            var ids = Enumerable.Range(StartId, CountIds).ToArray();

            var tasks = ids.Select(id => AsyncService.RunAsync(doc => DoWork(doc, id))).ToArray();
            Task.WaitAll(tasks);

            var changed = Container.Instances<SalesOrderHeader>().Where(soh => ids.Contains(soh.SalesOrderID)).ToArray();
            Assert.AreEqual(CountIds, changed.Length);
            changed.ForEach(soh => Assert.AreEqual(testMessage + soh.SalesOrderID, soh.Comment));
        }

        #endregion

        private void DoWork(IDomainObjectContainer container, int orderId) {
            var order = container.Instances<SalesOrderHeader>().Single(soh => soh.SalesOrderID == orderId);
            order.Comment = testMessage + orderId;
        }
    }
}