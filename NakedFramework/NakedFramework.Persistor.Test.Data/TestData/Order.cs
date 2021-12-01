// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace TestData; 

public class Order : TestHelper {
    [Key]
    public virtual int OrderId { get; set; }

    [Title]
    [Optionally]
    public virtual string Name { get; set; }

    public virtual int ParentOrderId { get; set; }
    public virtual bool PersistingCalled { get; set; }

    public override void Persisting() {
        base.Persisting();
        PersistingCalled = true;
    }

    public override void Persisted() {
        // cascade create more orders 
        base.Persisted();

        if (OrderId != 0 && OrderId < 5) {
            var newOrder = Container.NewTransientInstance<Order>();
            newOrder.ParentOrderId = OrderId;
            Container.Persist(ref newOrder);

            if (OrderId == 4) {
                Container.Instances<Order>().Single(i => i.OrderId == 1).Name = Guid.NewGuid().ToString();
            }
        }
    }
}