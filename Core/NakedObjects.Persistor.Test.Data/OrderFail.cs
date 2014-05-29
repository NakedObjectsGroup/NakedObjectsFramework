using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace TestData {
    public class OrderFail : TestHelper {
        [Key]
        public virtual int OrderFailId { get; set; }

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

            if (OrderFailId != 0 && OrderFailId < 5) {
                var newOrder = Container.NewTransientInstance<OrderFail>();
                newOrder.ParentOrderId = OrderFailId;
                Container.Persist(ref newOrder);

                if (OrderFailId == 4) {
                    throw new DomainException("Fail in Persisted");
                }
            }
        }
    }
}