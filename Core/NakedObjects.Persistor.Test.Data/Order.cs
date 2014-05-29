using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace TestData {
    public class Order : TestHelper {
        [Key]
        public virtual int OrderId { get; set; }

        [Title, Optionally]
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
}